from appstract.types import Page, Cluster
from typing import List
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
from sklearn.cluster import DBSCAN, KMeans, AgglomerativeClustering
from sklearn import metrics
from sklearn.decomposition import PCA
from sklearn.model_selection import GridSearchCV, cross_val_predict, train_test_split
from importlib import reload
from sklearn.base import TransformerMixin
import time
from tqdm import tqdm
from pprint import pprint
from statistics import mean
import math

DEFAULT_PARAMS = {
      'pca_features': {
            'branches': 15,
            'ids': 10,
            'classes': 10
      },
      'pca_final': 6,
      'tfidf': {
            'branches': False,
            'ids': True,
            'classes': True
      },
      'log': {
            'branches': False,
            'ids': False,
            'classes': False
      },
      # 'cluster_algo': 'kmeans'
      'cluster_algo': 'kmeans'
}

def branch_clustering(params = DEFAULT_PARAMS):
    def cluster(pages: List[Page]) -> List[Cluster]:



    return cluster
