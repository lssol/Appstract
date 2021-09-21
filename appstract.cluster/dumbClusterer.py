import appstract_pb2
import math

def dumbClusterer(pages):
    halfPages = math.floor((len(pages)/2))
    cluster1 = appstract_pb2.Cluster(pages = pages[::halfPages], confidence = 0.3)
    cluster2 = appstract_pb2.Cluster(pages = pages[halfPages::], confidence = 0.5)

    return appstract_pb2.ClusteringResponse(
        clusters = [cluster1, cluster2],
        confidence = 0.2
    )
