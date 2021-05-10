import puppeteer from 'puppeteer'

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
    await page.goto(url, {waitUntil: 'networkidle2'})
    await addBase(page)
    const html = await page.evaluate(() => document.documentElement.outerHTML)
    page.close()

    return html
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