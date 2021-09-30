import hashlib
from typing import List, Tuple, Set
from appstract.data.repository import Repository

def compute_tasks(clusterers) -> List[Tuple[str, str, str]]: # Each task is {applicationId, clusterer} 
    # a unit of work is: (applicationId, clusterer, pageHashes)
    repo = Repository()

    applications = repo.getApplicationsAndPageHashes()
    clusteringDone = repo.getClusteringDone()

    applications = map(projectApplication, applications)
    
    clustering_we_should_have = get_clustering_we_should_have(applications, clusterers)
    clustering_we_have = set(map(lambda x: (x['applicationId'], x['pagesHash'], x['clusterer']), clusteringDone))

    return list(clustering_we_should_have - clustering_we_have)

def projectApplication(app):
    return {
        'applicationId': app['_id'],
        'hash': hashArray(app['hashes']),
    }

def hashArray(array):
    return hashlib.sha256(''.join(array).encode()).hexdigest()

def get_clustering_we_should_have(applications, clusterers) -> Set[Tuple[str, str, str]]: # [(applicationId, pagesHash, clusterer)]
    res = set()
    for application in applications:
        for clusterer in clusterers:
            res.add((application['applicationId'], application['hash'], clusterer))
    return res

