from typing import List, Dict

from sklearn import cluster
from appstract.data.repository import Repository
from appstract.data.diff import compute_tasks
from appstract.clusterers.dumbClusterer import dumbClusterer
from appstract.types import Clusterer, Cluster, ClusteringResult, ClusteringResultDatabase
from appstract.clusterers.branch_clustering import branch_clustering

clusterers: Dict[str, Clusterer] = {
    "dumb": dumbClusterer,
    "branch": branch_clustering(),
}

def synchronize(repo: Repository):
    print("Starting Synchronization...")
    tasks = compute_tasks(clusterers.keys())
    print(f'{len(tasks)} clustering tasks to synchronize...')

    for appId, pagesHash, clusterer_name in tasks:
        print(f'Synchronizing {appId} with {clusterer_name}')
        clusterer = clusterers[clusterer_name]
        print("Getting pages from app ", appId)
        pages = repo.getPagesFromAppId(appId)
        print(f'Retrieved {len(pages)} pages')
        resultClustering = clusterer(pages)
        displayClusters(resultClustering)
        repo.saveClustering(toDatabaseFormat(resultClustering, pagesHash, appId, clusterer_name))

def displayClusters(res: ClusteringResult):
    clusters = res.clusters
    for i, cluster in enumerate(clusters):
        print(f'Cluster {i}')
        for page, silhouette in cluster.pages:
            print(f'[{silhouette}] {page.url}')

def toDatabaseFormat(res: ClusteringResult, pagesHash, appId, clusterer) -> ClusteringResultDatabase:
    clusters = []
    for cluster in res.clusters:
        c = []
        for page, silhouette in cluster.pages:
            c.append({
                'url': page.url,
                'silhouette': silhouette
            })
        clusters.append(c)

    return ClusteringResultDatabase(
        silhouette = res.silhouette,
        pagesHash = pagesHash,
        applicationId = appId,
        domain = "",
        nb_clusters = len(clusters),
        clusterer = clusterer,
        clusters = clusters
    )