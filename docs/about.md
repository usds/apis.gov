---
layout: page
title: About APIs.gov
---

## What is this?
APIs.gov is directory and registry for Application Programming Interfaces (APIs) created or maintained by agencies within the federal government. Here you will find a searchable list for dicovering these APIs, as well as a hosted machine-readable list of APIs in our <a href="/apis.json">apis.json</a> file.

## What can I do here?
You can discover government APIs published by numerous agencies within the federal ecosystem. Additionally, you can search by keyword and get basic information about each API, such as the link to documentation and the point of contact for the API.

You can also submit APIs to be added to the registry.

## What APIs are in the registry?
Check them out <a href="/apis.gov/">here!</a> You can also find the machine-readable format <a href="/apis.json">here</a>.

## How do I submit an API to the registry?
There are two ways to get APIs added to this registry. By creating an apis.json file and hosting it at the root of your agency web site, we can scrape thd information and add it automatically. You can also submit your API to us with <a href="https://usds.github.io/apis.gov/submit.html">this form</a>. In either case, a human reviewer will receive the submission and approve them for addition to the registry.

Here's an example of how to format your own apis.json file.
```json
{
  "name": "APIs.gov",
  "description": "This is an inventory of US government APIs currntly maintained by the US Digital Service.",
  "image":"https://www.usds.gov/assets/img/usds-logo-footer.svg",
  "tags": [
    "government",
    "API",
    "US",
    "US Digital Service",
    "USDS"
  ],
  "created": "2020-03-05",
  "modified": "220-03-05",
  "url": "https://usds.github.io/apis.gov/apis.json",
  "specificationVersion": "0.15",
  "apis": [
    {
      "name": "Data at the Point of Care",
      "description": "Enables healthcare providers to make a patient’s Medicare claims data available to the provider for a patient’s treatment needs.",
      "image": "https://dpc.cms.gov/assets/top-nav-heart-5fe16cc0fca21cb303b25f95b38070b7d93f452f09e9892eec874353a4482988.svg",
      "humanURL": "https://dpc.cms.gov/",
      "baseURL": "https://sandbox.dpc.cms.gov/api",
      "tags": [
        "medicare",
        "claims",
        "health",
        "cms",
        "fhir",
        "hhs"
      ],
      "properties": [
        {
          "type": "X-signup",
          "url": "https://dpc.cms.gov/users/sign_up"
        },
        {
          "type": "OpenAPI",
          "url": "https://sandbox.dpc.cms.gov/api/swagger"
        },
        {
          "type": "FHIR-IG",
          "url": "https://dpc.cms.gov/ig/index.html"
        }
      ],
      "contact": [
        {
          "email": "info@dpc.cms.gov"
        }
      ]
    }
  ],
  "maintainers": [
    {
      "FN": "Shelby",
      "email": "shelby.switzer@cms.hhs.gov"
    }
  ]
}```
