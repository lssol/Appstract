const urlApi = 'http://localhost:5000'
const urlRobot = 'http://localhost:3000'

const checkForErrors = (response) => {
    if (!response.ok)
    {
        console.log("Call to API returned an error")
        console.log(response)
        throw "Error when calling API"
    }

    return response
}

const get = async (endpoint, params = {}) => {
    const url = new URL(endpoint)
    for (let key in params)
        url.searchParams.set(key, params[key])
    try {
        let response = await fetch(url, { method: 'GET', })
        response = checkForErrors(response)
        return await response.json()
    } catch (e) {
        console.log("Failed to contact API")
        console.log(e)
        return null
    }
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
        const res = await send(`${urlApi}/application`, 'POST')
        return await res.json()
    },

    getApplication: async function(id) {
        return await get(`${urlApi}/application`, { applicationId: id })
    },
    
    getApplications: async function() {
        return await get(`${urlApi}/applications`)
    },
    
    removeApplication: async function(id) {
        return await send(`${urlApi}/application`, 'DELETE', {applicationId: id})
    },
    
    renameApplication: async function(id, name) {
        return await send(`${urlApi}/application/rename`, 'POST', {applicationId: id, newName: name})
    },

    removeTemplate(templateId) {
        return send(urlApi + '/template/remove', 'DELETE', {templateId: templateId})
    },

    createTemplate: async function(applicationId) {
        const res = await send(`${urlApi}/template`, 'POST', {applicationId: applicationId})
        return await res.json()
    },
    
    renameTemplate: async function(templateId, name) {
        return await send(`${urlApi}/template/rename`, 'POST', {templateId: templateId, newName: name})
    },
    
    async createModel(applicationId, pages) {
        return await send(`${urlApi}/application/model`, 'POST', {applicationId, pages})
    },
    
    setUrlTemplate: async function(templateId, url) {
        const res = await send(`${urlApi}/template/url`, 'POST', {templateId: templateId, url: url})
        if (res == null)
            throw "Invalid Url"
        return await res.json()
    }
}
