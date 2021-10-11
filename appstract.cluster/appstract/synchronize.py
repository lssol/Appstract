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
    yield "Starting Synchronization..."
    tasks = compute_tasks(clusterers.keys())
    yield f'{len(tasks)} clustering tasks to synchronize...'

    for appId, pagesHash, clusterer_name in tasks:
        yield '---------------------------------'
        yield f'Synchronizing {appId} with {clusterer_name}'
        clusterer = clusterers[clusterer_name]
        yield f'Getting pages from app {appId}'
        pages = repo.getPagesFromAppId(appId)
        yield f'Retrieved {len(pages)} pages'
        resultClustering = clusterer(pages)
        # displayClusters(resultClustering)
        repo.saveClustering(toDatabaseFormat(resultClustering, pagesHash, appId, clusterer_name))
        yield f'Successfully computed and saved {len(resultClustering.Clusters)} clusters for app {appId}'
    
    yield "------------------------------------"
    yield "Synchronization done"

def displayClusters(res: ClusteringResult):
    clusters = res.Clusters
    for i, cluster in enumerate(clusters):
        print(f'Cluster {i}')
        for page, silhouette in cluster.Pages:
            print(f'[{silhouette}] {page.Url}')

def toDatabaseFormat(res: ClusteringResult, pagesHash, appId, clusterer) -> ClusteringResultDatabase:
    clusters = []
    for cluster in res.Clusters:
        c = []
        for page, silhouette in cluster.Pages:
            c.append({
                'Url': page.Url,
                'Silhouette': silhouette
            })
        clusters.append(c)

    return ClusteringResultDatabase(
        Silhouette = res.Siohouette,
        PagesHash = pagesHash,
        ApplicationId = appId,
        Domain = "",
        NbClusters = len(clusters),
        Clusterer = clusterer,
        Clusters = clusters,
        Outdated = False
    )