from concurrent import futures
import logging
import grpc
import appstract.__rpc.appstract_pb2 as appstract_pb2
import appstract.__rpc.appstract_pb2_grpc as appstract_pb2_grpc
from appstract.data.repository import Repository
from appstract.synchronize import synchronize

class Clustering(appstract_pb2_grpc.ClusteringServicer):
    def StartClustering(self, request, context):
        repo = Repository()
        for message in synchronize(repo):
            print(message)
            yield appstract_pb2.Progress(message=message)

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    appstract_pb2_grpc.add_ClusteringServicer_to_server(Clustering(), server)
    port = "50052"
    server.add_insecure_port('[::]:' + port)
    server.start()
    print("Listening on port " + port)
    server.wait_for_termination()

if __name__ == '__main__':
    logging.basicConfig()
    serve()
