import repository
import hashlib
from typing import List, Tuple, Set

def compute_tasks(clusterers) -> List[Tuple[str, str]]: # Each task is {applicationId, clusterer} 
    # a unit of work is: (applicationId, clusterer, pageHashes)
    repo = repository.Repository()

    applications = repo.getApplicationsAndPageHashes()
    clusteringDone = repo.getClusteringDone()

    applications = map(projectApplication, applications)
    
    clustering_we_should_have = get_clustering_we_should_have(applications, clusterers)
    clustering_we_have = set(map(lambda x: (x['applicationId'], x['hash'], x['clusterer']), clusteringDone))
    tasks = clustering_we_should_have - clustering_we_have

    return [(appId, clusterer) for (appId, pagesHash, clusterer) in tasks]

def projectApplication(app):
    return {
        'applicationId': app['_id'],
        'hash': hashArray(app['applicationId']),
    }

def hashArray(array):
    return hashlib.sha256(b''.join(array)).hexdigest()

def get_clustering_we_should_have(applications, clusterers) -> Set[Tuple[str, str, str]]: # [(applicationId, pagesHash, clusterer)]
    res = set()
    for application in applications:
        for clusterer in clusterers:
            res.add((application['applicationId'], application['hash'], clusterer))
    return res

