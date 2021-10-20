'use strict';
import puppeteer from 'puppeteer'
// import {removeOverlay} from './removeOverlay.js'

let browser

const getBrowser = async () => {
    if (browser && browser.isConnected())
        return browser

    browser = await puppeteer.launch()
    return browser
}

export const urlToHtml = async (url) => {
    const browser = await getBrowser()
    const page = await browser.newPage()

    try {
        await page.goto(url, { waitUntil: 'networkidle2' })
    } catch (e) {
        page.close()
        throw "Invalid url"
    }

    await addBase(page)
    await removeOverlayOnPage(page)
    await addSignature(page)

    const html = await page.evaluate(() => document.documentElement.outerHTML)

    page.close()

    return html
}

const getHostname = (url) => {
    try {
        let h = new URL(url).hostname
        return h.replace(/^www\./, '')
    } catch (e) {
        return ""
    }
}

export const explore = async (handles, options) => {
    const { domain, depth, width, delay, minNodes } = options

    const hostname = getHostname(domain)
    const browser = await getBrowser()
    const page = await browser.newPage()
    stopRedirect(page)

    page.setDefaultNavigationTimeout(10000)

    try {
        await page.goto(domain, { waitUntil: 'networkidle0' })
    } catch (e) {
        page.close()
        console.error("Error when opening entry page. The explorer will stop.", e)
        throw "Invalid url"
    }

    let taskQueue = [{ url: domain, depth: 0, origin: null }]
    let alreadyVisited = new Set()

    while (taskQueue.length > 0) {
        let { url, depth: currentDepth, origin } = taskQueue.shift()
        alreadyVisited.add(url)

        console.log(`[depth=${currentDepth}] Exploring ${url}`)

        try {
            await page.goto(url, { waitUntil: 'networkidle2', timeout: 5000 })
            console.info("Page loaded")

            let links = await page.evaluate(() => Array.from(document.documentElement.querySelectorAll('a')).map(e => e.href))
            links = links.filter(l => (getHostname(l) === hostname) && !alreadyVisited.has(l))

            let pageResult = await getPageResult(page)
            let res = { ...pageResult, origin, domain, nbLinks: links.length }

            if (res.nbNodes < minNodes)
                throw `Url was ignored because it has less than ${minNodes} nodes": ${res}`

            handles.success(res)

            if (currentDepth == depth) {
                console.log("Reached depth limit")
                continue
            }

            const nextLinks = shuffleArray(links).slice(0, width)
            const newTasks = nextLinks.map(link => ({ url: link, depth: currentDepth + 1, origin: url }))
            taskQueue.push(...newTasks)
        } catch (e) {
            handles.failure(url)
            console.warn(`Error while loading ${url}`, e)
        } finally {
            await new Promise(r => setTimeout(r, delay)) // SLEEP
        }
    }

    console.log(`Finished exploring`)
    page.close()
}

const stopRedirect = async page => {
    await page.setRequestInterception(true);

    page.on('request', request => {
        var redirectChain = request.redirectChain().map(r => r.url()).reverse();
        var shouldAbort =
            request.isNavigationRequest() &&
            redirectChain.length !== 0 &&
            getHostname(redirectChain[0]) !== getHostname(request.url());

        if (shouldAbort) {
            console.log("Aborting due to redirection: ", redirectChain)
            request.abort();
        } else {
            request.continue();
        }
    });
}

const shuffleArray = (array) => {
    let arr = array.slice()
    for (let i = arr.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [arr[i], arr[j]] = [arr[j], arr[i]];
    }
    return arr
}

const getPageResult = async (page) => {
    await addBase(page)
    // await removeOverlayOnPage(page)
    await addSignature(page)

    const content = await page.evaluate(() => document.documentElement.outerHTML)
    const nbNodes = await page.evaluate(() => document.documentElement.querySelectorAll('*').length)
    const url = await page.url()
    return { content, nbNodes, url }
}


const addBase = (page) => {
    return page.evaluate(() => {
        const url = window.location
        const baseUrl = url.protocol + '//' + url.host
        const baseTag = document.createElement('base')
        baseTag.setAttribute('href', baseUrl)
        document.head.appendChild(baseTag)
    })
}

const removeOverlayOnPage = async (page) => {
    return page.evaluate(async () => {
        var debug = false;

        var utils = (function () {
            function hideElement(element) {
                styleImportant(element, 'display', 'none');
            }

            function styleImportant(element, cssProperty, cssValue) {
                element.style[cssProperty] = '';
                var cssText = element.style.cssText || '';
                if (cssText.length > 0 && cssText.slice(-1) != ';')
                    cssText += ';';
                // Some pages are using !important on elements, so we must use it too
                element.style.cssText = cssText + cssProperty + ': ' + cssValue + ' !important;';
            }

            function isVisible(element) {
                return element.offsetWidth > 0 && element.offsetHeight > 0;
            }

            function getZIndex(element) {
                return parseInt(window.getComputedStyle(element).zIndex);
            }

            function isAnElement(node) {
                return node.nodeType == 1; // nodeType 1 mean element
            }

            function nodeListToArray(nodeList) {
                return Array.prototype.slice.call(nodeList);
            }

            function forEachElement(nodeList, functionToApply) {
                nodeListToArray(nodeList).filter(isAnElement).forEach(function (element) {
                    functionToApply.call(this, element);
                });
            }

            function collectParrents(element, predicate) {
                var matchedElement = element && predicate(element) ? [element] : [];
                var parent = element.parentNode;

                if (parent && parent != document && parent != document.body) {
                    return matchedElement.concat(collectParrents(parent, predicate));
                } else {
                    return matchedElement;
                }
            }

            // Calculate the number of DOM elements inside an element
            function elementWeight(element, maxThreshold) {
                var grandTotal = 0;
                var nextElement = element;
                var nextGrandChildNodes = [];

                function calculateBreathFirst(element) {
                    var total = 0;
                    var nextChildElements = [];

                    var childNodes = element.childNodes;
                    total = childNodes.length;

                    forEachElement(childNodes, function (childNode) {
                        var grandChildNodes = nodeListToArray(childNode.childNodes);
                        total += grandChildNodes.length;
                        nextChildElements = nextChildElements.concat(grandChildNodes.filter(isAnElement));
                    });
                    return [total, nextChildElements];
                }

                while (nextElement) {
                    var tuple_total_nextChildElements = calculateBreathFirst(nextElement);
                    var total = tuple_total_nextChildElements[0];

                    grandTotal += total;
                    nextGrandChildNodes = nextGrandChildNodes.concat(tuple_total_nextChildElements[1]);

                    if (grandTotal >= maxThreshold) {
                        break;
                    } else {
                        nextElement = nextGrandChildNodes.pop();
                    }
                }

                return grandTotal;
            }

            return {
                hideElement: hideElement,
                isVisible: isVisible,
                getZIndex: getZIndex,
                forEachElement: forEachElement,
                collectParrents: collectParrents,
                elementWeight: elementWeight,
                styleImportant: styleImportant
            }
        })();

        var overlayRemover = function (debug, utils) {
            function hideElementsAtZIndexNear(nearElement, thresholdZIndex) {
                var parent = nearElement.parentNode;
                // The case when nearElement is a document
                if (parent === null) {
                    return;
                }
                var children = parent.childNodes;

                utils.forEachElement(children, function (child) {
                    if (utils.getZIndex(child) >= thresholdZIndex) {
                        utils.hideElement(child);
                    }
                })
            }

            // Check the element in the middle of the screen
            // Search fo elements that have zIndex attribute
            function methodTwoHideElementMiddle() {
                var overlayPopup = document.elementFromPoint(window.innerWidth / 2, window.innerHeight / 2);

                var overlayFound = utils.collectParrents(overlayPopup, function (el) {
                    return utils.getZIndex(el) > 0;
                });

                if (debug)
                    console.debug('Overlay found: ', overlayFound);

                if (overlayFound.length == 0)
                    return false;

                var olderParent = overlayFound.pop();

                if (debug)
                    console.debug('Hide parrent: ', olderParent);

                return olderParent;
            }

            function disableBlur() {
                var someContainerMaybe = document.elementFromPoint(window.innerWidth / 2, window.innerHeight / 2);

                var bluredParentsFound = utils.collectParrents(someContainerMaybe, function (el) {
                    return window.getComputedStyle(el).filter.includes('blur');
                });

                if (bluredParentsFound.length == 0)
                    return false;

                var topParent = bluredParentsFound.pop();

                // Some element can act as a container, that can be blured or masking the whole content
                var isContainerOccupyingAboutSpaceAsBody = topParent.offsetWidth >= (document.body.offsetWidth - 100);

                if (isContainerOccupyingAboutSpaceAsBody) {
                    utils.styleImportant(topParent, 'filter', 'blur(0)');

                    if (debug) console.log('Blur removed!', topParent);

                    return true;
                }

                return false;
            }

            function containersOverflowAuto() {
                var containers = [document.documentElement, document.body];

                containers.forEach(function (element) {
                    if (window.getComputedStyle(element).overflow == 'hidden') {
                        utils.styleImportant(element, 'overflow', 'auto');
                    }
                    if (window.getComputedStyle(element).position == 'fixed') {
                        utils.styleImportant(element, 'position', 'static');
                    }
                })
            }

            function run() {
                for (var i = 0; i < 10; i++) {
                    var candidate = methodTwoHideElementMiddle();
                    var first = i == 0;
                    if (candidate === false) {
                        break;
                    } else {
                        if (!first) {
                            // Prevent to hide the actual content
                            var weightThreshold = 100;
                            var candidateWeight = utils.elementWeight(candidate, weightThreshold)
                            if (candidateWeight < weightThreshold) {
                                if (debug)
                                    console.log('Element is too lightweigh, hide it', candidate);
                                utils.hideElement(candidate);
                            } else {
                                if (debug)
                                    console.log("Element is too heavy, don't hide it", candidate);
                            }
                        } else {
                            utils.hideElement(candidate);
                            containersOverflowAuto();
                            disableBlur();
                        }
                    }
                }
            }

            return {
                run: run
            };

        };

        const overlayRemoverInstance = overlayRemover(debug, utils);
        overlayRemoverInstance.run();
    })
}

const addSignature = async (page) => {
    return page.evaluate(async () => {
        const genString = () => 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => { let r = Math.random() * 16 | 0; let v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        const elements = document.querySelectorAll("*")
        elements.forEach(e => e.setAttribute('signature', genString()))
    })
}