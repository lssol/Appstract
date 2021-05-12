import express from 'express'
import {urlToHtml} from './robot.js'
import cors from 'cors'

const app = express()
app.use(cors())
const port = 3000

app.get('/urltohtml', async (req, res, next) => {
  const url = req.query.url
  if (!url) {
    console.log('No url provided')
    res.status(400)
    res.json({error: 'No url provided'})
    next()
  }
  console.log("Retrieving html for url", url)

  try {
    const html = await urlToHtml(req.query.url)
    res.json({html})
  }
  catch (e) {
    console.log(e)
    res.status(500)
    res.json({error: e.toString()})
  }
})

app.listen(port, () => {
  console.log(`Robot listening at http://localhost:${port}`)
})