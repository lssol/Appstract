const urlApi = 'http://localhost:5000'

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

// Send page -> receive: signature -> name
