import puppeteer from 'puppeteer'
import {removeOverlay} from './removeOverlay.js'

let browser

const getBrowser = async () => {
    if (browser)
        return browser

    browser = await puppeteer.launch()
    return browser
}

export const urlToHtml = async (url) => {
    const browser = await getBrowser()
    const page = await browser.newPage()
    
    try {
        await page.goto(url, {waitUntil: 'networkidle2'})
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

export const explore = async (domain, depth, width, delay, minNodes) => {
    const browser = await getBrowser()
    const page = await browser.newPage()
    try {
        await page.goto(url, {waitUntil: 'networkidle2'})
    } catch (e) {
        page.close()
        throw "Invalid url"
    } 

    let pagesReturned = []
    let errors = []
    let taskQueue = [{url: domain, depth: 0, origin: null}]

    let rec = () => {
        if (taskQueue.length == 0) {
            console.log("No more tasks")
            return
        }

        let {url, currentDepth, origin} = taskQueue[0]

        if (currentDepth > depth) {
            console.log("Reached depth limit")
            return
        }

        console.log("Exploring " + url)

        try {
            await page.goto(url, {waitUntil: 'networkidle2'})
            let res = {...pagesReturned.push(getPage(page)), origin, domain}
            if (res.nbNodes > minNodes) {
                console.Warn(`Url: ${url} has`)
            }

            const nextLinks = await page.evaluate(() => { 
                let links = Array.from(document.querySelectorAll('a')).map(a => a.href)
                return shuffleArray(links).take(width)
            })

            Array.from(nextLinks)
                .map(link => taskQueue.push({url: link, depth: currentDepth + 1, origin: url}))

        } catch (e) {
            errors.push(url)
            console.warn(`Error while loading ${url}`)
        } 
        

    }
    
    rec
    page.close()

    return html
}

const shuffleArray = (array) => {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
}

const getPage = (page) => {
    await addBase(page)
    await removeOverlayOnPage(page)
    await addSignature(page)
    
    const content = await page.evaluate(() => document.documentElement.outerHTML)
    const nbLinks = await page.evaluate(() => document.documentElement.querySelectorAll('a').length)
    const nbNodes = await page.evaluate(() => document.documentElement.querySelectorAll('*').length)
    const url     = await page.url()
    return { content, nbLinks, nbNodes, url }
}

const genString = () => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0
        var v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
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
                        if (first)
                            alert('No overlay has been found on this website.');
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
    await page.exposeFunction("genString", genString)
    return page.evaluate(async () => {
        const elements = document.querySelectorAll("*")
        elements.forEach(async (e) => e.setAttribute('signature', await genString()))
    })
}