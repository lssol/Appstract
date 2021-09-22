from concurrent import futures
import logging

import grpc

import _rpc.appstract_pb2 as appstract_pb2
import _rpc.appstract_pb2_grpc as appstract_pb2_grpc
import dumbClusterer

clusterers = {
    "Dumb_Clusterer": dumbClusterer,
    "Branch_Clusterer": dumbClusterer,
}

class Clustering(appstract_pb2_grpc.ClusteringServicer):
    def CreateClustering(self, request, context):
        clusterer = clusterers[request.clusterer]
        domain = request.domain
        pages = request.pages

        return appstract_pb2.ClusteringResponse()

    def GetClusterers(self, request, context):
        return appstract_pb2.ClusterersReply(labels=clusterers.keys())

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
