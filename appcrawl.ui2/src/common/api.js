const baseUrl = 'http://localhost:5000'

const checkForErrors = (response) => {
    if (!response.ok)
    {
        console.log("Call to API returned an error")
        console.log(response)
        return null
    }

    return response.json()
}

const post = async (endpoint, data = {}) => {
    let response
    try {
        response = await fetch(`${baseUrl}/${endpoint}`, {
            method: 'POST',
            mode: 'no-cors',
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

const get = async (endpoint, params = {}) => {
    const url = new URL(`${baseUrl}/${endpoint}`)
    for(let key in params)
        url.searchParams.set(key, params[key])
    let response
    try {
        response = await fetch(url, { 
            method: 'GET',
            mode: 'no-cors',
        })
    } catch(e) {
        console.log("Failed to contact API")
        console.log(e)
        return null
    }

    return checkForErrors(response)
}

export default {
    createApplication: function() {
        return post('application')
    },

    getApplication: function(id) {
        return get('application', { applicationId: id })
    }    
} 