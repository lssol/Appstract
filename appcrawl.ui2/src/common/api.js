const baseUrl = 'http://localhost:5000'

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
    const url = new URL(`${baseUrl}/${endpoint}`)
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
        response = await fetch(`${baseUrl}/${endpoint}`, {
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
        return await send('application', 'POST')
    },

    getApplication: async function(id) {
        return await get('application', { applicationId: id })
    },
    
    renameApplication: async function(id, name) {
        return await send('application/rename', 'POST', {applicationId: id, newName: name})
    }
} 
