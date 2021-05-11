const urlApi = 'http://localhost:5000'
const urlRobot = 'http://localhost:3000'

const checkForErrors = async (response) => {
    if (!response.ok)
    {
        console.log("Call to API returned an error")
        console.log(response)
        return null
    }

    return await response.json()
}

const get = async (endpoint, params = {}) => {
    const url = new URL(endpoint)
    for (let key in params)
        url.searchParams.set(key, params[key])
    let response
    try {
        response = await fetch(url, { method: 'GET', })
    } catch (e) {
        console.log("Failed to contact API")
        console.log(e)
        return null
    }

    return checkForErrors(response)
}

const send = async (endpoint, method, data = {}) => {
    let response
    try {
        response = await fetch(endpoint, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: data ? JSON.stringify(data) : ""
        })
    } 
    catch(e) {
        console.log("Failed to contact API")
        console.log(e)
        return null
    }

    return checkForErrors(response)
}

export default {
    createApplication: async function() {
        return await send(`${urlApi}/application`, 'POST')
    },

    getApplication: async function(id) {
        return await get(`${urlApi}/application`, { applicationId: id })
    },

    getHtml: async function(url) {
        return await get(`${urlRobot}/urltohtml`, { url: url })
    },
    
    renameApplication: async function(id, name) {
        return await send(`${urlApi}/application/rename`, 'POST', {applicationId: id, newName: name})
    },

    removeTemplate(templateId) {
        return send(urlApi + '/template/remove', 'DELETE', {templateId: templateId})
    },

    createTemplate: async function(applicationId) {
        return await send(`${urlApi}/template`, 'POST', {applicationId: applicationId})
    },
    renameTemplate: async function(templateId, name) {
        return await send(`${urlApi}/template/rename`, 'POST', {templateId: templateId, newName: name})
    },
    setUrlTemplate: async function(templateId, url) {
        return await send(`${urlApi}/template/url`, 'POST', {templateId: id, url: url})
    }
}
