from pymongo import MongoClient
from typing import List, Tuple

class Repository:
    def __init__(self):
        uri = 'mongodb://localhost:27017'
        self.client = MongoClient(uri)
        self.applications = self.client.appstract_front.Application
        self.pages = self.client.appstract_front.Page
        self.clustering = self.client.appstract_front.Clustering

    def getApplicationsAndPageHashes(self): 
        return self.pages.aggregate(
            [{
            "$group" : 
                {"_id" : "$ApplicationId", 
                "hashes" : {"$addToSet" : "$PageHash"}
                }}
            ]) # [{_id: 'applicationID', hashes: ['hash1', 'hash2', ...]}]

    def getClusteringDone(self): # Each clustering done is: {applicationId, clusterer, pagesHash}
        return self.clustering.find({}, {'applicationId': 1, 'clusterer': 1, 'pagesHash': 1})

    def getPagesFromAppId(self, applicationsId):
        return self.pages.find({'ApplicationId': applicationsId}, {'Content': 1, '_id': 0})

