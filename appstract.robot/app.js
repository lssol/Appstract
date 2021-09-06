import express from 'express'
import {urlToHtml} from './robot.js'
import cors from 'cors'

const app = express()
app.use(cors())
const port = 3000

app.get('/urltohtml', async (req, res) => {
  try {
    const url = req.query.url
    console.log("Attempting to retrieve HTML for url: ", url)
    const html = await urlToHtml(url)
    console.log("Successfully retrieved HTML for url: ", url)
    res.json({html})
  }
  catch (e) {
    console.log(e)
    res.status(400)
    res.json({error: e.toString()})
  }
})

app.listen(port, () => {
  console.log(`Robot listening at http://localhost:${port}`)
})