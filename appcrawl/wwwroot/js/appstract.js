var appstract = (function () {
    let Appstract = () => {
        let host = "http://localhost:5000/"
        let POST = 'POST'

        let sendRequest = (method, endpoint, body) => {
            return new Promise((resolve, reject) => {
                let xhr = new XMLHttpRequest()
                xhr.open(method, host + endpoint)
                xhr.setRequestHeader('Content-Type', 'application/json')

                let json = JSON.stringify(body)
                xhr.send(json)

                xhr.onload = () => resolve(JSON.parse(xhr.response))
                xhr.onerror = () => reject(xhr.response)
            })
        }

        return {
            abstractPage: (page) => sendRequest(POST, 'intra', {src: page}),
            identify: (page, host) => sendRequest(POST, 'application/identify', {page: page, host: host})
        }
    }
    let DOM = {
        selectElements: (elems, color = `#ffd460`) => {
            let selectElement = (elem) => {
                const originalBackground   = elem.style.backgroundColor
                elem.style.backgroundColor = color

                return () => elem.style.backgroundColor = originalBackground
            }
            const unselects = elems.map(e => selectElement(e))
            return {
                unselect: () => unselects.forEach(unselect => unselect())
            }
        },
    }

    let setSignatures = (body) => {
        body.querySelectorAll('*').forEach((el, i) => el.setAttribute('signature', i.toString()))
    }

    let idToColor = (id) => {
        function hashCode(str) { // java String#hashCode
            let hash = 0;
            for (let i = 0; i < str.length; i++) {
                hash = str.charCodeAt(i) + ((hash << 5) - hash);
            }
            return hash;
        }

        function intToRGB(i) {
            const c = (i & 0x00FFFFFF)
                .toString(16)
                .toUpperCase();

            return "00000".substring(0, 6 - c.length) + c;
        }

        return '#' + intToRGB(hashCode(id))
    }
    
    const getHostname = () => {
        const url = window.location.href
        let h = new URL(url).hostname
        return h.replace(/^www\./, '')
    }
    
    let identify = async () => {
        let body = document.body
        setSignatures(body)
        console.log("Starting the identification...")
        const host = getHostname()
        console.log(`Searching for host: ${host}`)
        let response = await Appstract().identify(body.outerHTML, getHostname())
        console.log("Identification done", response)
        let signatureToColor = new Map(Array.from(response.ids).map(({signature: s, id: id}) => [s, idToColor(id)]))
        let unselect = () => {}
        document.body.querySelectorAll('*').forEach(e => e.addEventListener("mouseenter", (evt) => {
            if (!evt.altKey)
                return

            unselect()
            let signature = e.getAttribute('signature')
            if (signatureToColor.has(signature))
                unselect = DOM.selectElements([e], signatureToColor.get(signature)).unselect
        }))
    }

    return {
        identify: identify,
    }
}())
