from typing import List
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler, FunctionTransformer
from sklearn.feature_extraction.text import CountVectorizer, TfidfTransformer, TfidfVectorizer
from sklearn.base import TransformerMixin
from sklearn.decomposition import PCA
import numpy as np


class DenseTransformer(TransformerMixin):
    def fit(self, X, y=None, **fit_params):
        return self

    def transform(self, X, y=None, **fit_params):
        return X.todense()


def get_preprocessor(feature, pca_dim: int, tfidf: bool, log: bool):
    steps = [('vect', CountVectorizer(ngram_range=(1,1), analyzer = "word", tokenizer = lambda x: x.split(), preprocessor = None, stop_words = None, max_df=0.9, min_df=5, max_features = 1000))]

    if tfidf == True:
        steps.append(('tfidf', TfidfTransformer()))

    steps.append(('dense', DenseTransformer()))

    if log == True:
        steps.append(('log', FunctionTransformer(np.log1p, validate=True)))
    
    steps.append(('scaler', StandardScaler()))
    steps.append(('pca', PCA(pca_dim)))

    return Pipeline(steps)