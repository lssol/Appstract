import {explore} from './robot.js';
import grpc from '@grpc/grpc-js'
import protoLoader from '@grpc/proto-loader'
import { dirname } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
let PROTO_PATH = __dirname + '/../robot.proto';
let packageDefinition = protoLoader.loadSync(
    PROTO_PATH,
    {keepCase: true,
     longs: String,
     enums: String,
     defaults: true,
     oneofs: true
    });

let rpc = grpc.loadPackageDefinition(packageDefinition).robot;

/**
 * Implements the SayHello RPC method.
 */
async function Explore(call) {
    const {domain} = call.request;
    console.log("Received explore call with domain: " + domain)

    try {
      await explore({
          success: (res) => {call.write(res)},
          failure: (url) => {call.write({url, error: true})}
      }, {domain, depth: 3, width: 3, delay: 500, minNode: 35})
    }
    catch (e) {
      call.end()
    }

    call.end()
    console.log("Finished exploration of " + domain)
}

/**
 * Starts an RPC server that receives requests for the Greeter service at the
 * sample server port
 */
function main() {
  let server = new grpc.Server();
  let port = 50051;
  server.addService(rpc.Robot.service, {Explore: Explore});
  server.bindAsync('0.0.0.0:' + port, grpc.ServerCredentials.createInsecure(), () => {
    server.start();
    console.log("Started the server at port: " + port)
  });
}

main();
