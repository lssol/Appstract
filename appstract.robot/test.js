import {explore} from './robot.js'

let res = await explore("https://dealabs.com", 1, 2, 500, 35)
console.log(res)