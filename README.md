# Appstract

This API exposes the appstract functionalities.
Appstract allows to create and use an abstraction of any web app.

## API

In order to communicate between the server and the client, we use signatures attributes.
The client is expected to add signature attributes to the DOM before sending it to the API.

We use these signatures to reference the original DOM tree.

The main use cases of Appstract are:
- Information Extraction
- Web analytics
- Testing / Crawling

### Information Extraction

For information extraction, we use the concept of data-tree.

#### Record Extraction

We rely on the repetition of patterns within the DOM to extract records.

```
DOM with signature -> Records 
```
The Records are provided under the form of a tree
The leaves of the tree are original text nodes
The nodes inside are "boxes".

#### Inter Page Abstraction

The full app abstraction (appstraction ;P) is done in two steps:
1) Model creation

*For the model creation*, you need to provide one page per type of page.
We then: 
- Apply intra page abstraction to each page
- For each page, for each element, we assign special identifiers we call `app_id` the idea is that these ids identify elements across records and pages
- Reduce the set of pages to one canon app page by successively matching the pages
    ```
    let merge page1 page2 =
        let matching = match page1 page2
        return page1, but with only the elements that had a match in page2

    pages |> reduce merge
    ```
    Doing so allows us to get a canonical page of the site.
- Match all pages with the canonical page and replace the `app-id` of each matched element by the `app-id` of the canonical page.

Note: 
This process is theoretically recursive.
In the future, we will allow to feed a hierarchy of pages (allowing nested clusters).

2) Extract information from a page 
- We receive a page (with signature)
- We intra-page-abstract it
- We match it with each of the template pages
- We select the one template page that had the best matching score
- We determine the `app_id` on the new page for each element using the matching to the template

For web analytics and crawling we can just return the original DOM (with signature) with a mapping: `signature -> app-id`

For information extraction, we check which `app-id` are "boxes" and return the data-tree




## Cohesion

Problem: 
Imagine two product pages

There are many things that match (and whose content change) between two pages.

Each of the elements whose different content have changed will be considered as information by the algorithm.

The problem is that unlike for the records, we don't have the structure to help us understand what pieces of information should be grouped together




## AppCrawl

There are several parts to the crawler:

### UI

### API
- CRUD operations on apps/templates/elements
- Create Model
  - Call appstract to create model from the set of templates
- Create Elements (only if the model has been created)

### Robot
For each command, there is a specific file describing what to do.

Sync
```
    GetHTML(Starting Point, Elem[])
```

Async

```
    Crawl(Starting Point, Elem[])
```



