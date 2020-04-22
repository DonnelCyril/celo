# Solution Design

## Assumptions

I have made some assumptions while designing the API.

* I have assumed that the API will be read heavy and is optimised for this use case.
* The API need to handle large call volumes while keeping a minimal operational cost.

* I **havent** tried to replicate the exact structure / functionality of the randomuser api, instead the models have been defined to best support the above use case.

* The implementation focuses mainly<br>
  * on how we could leverage a CDN cache service to meet large call volumes
  * how the Mediator design pattern was leveraged in building the API functionality
  * how exceptions and validation errors are handled inside the app
  * how cross cutting concerns like logging, metrics, in-memory caching etc can be enforced consistently across the app. I feel the above provide a good foundation to further build upon the API functionality.

* To fit within time constraints:<br>
    * I haven't done an end to end integration with external services like Prometheus, Azure KeyVault, CDN's cache purge API etc. A stub implementation has been used in these cases. I have still demonstrated how this will be used in a real world app.
    * I have demonstrated the different testing patterns I would use but hasn't written test for every use case. Tests for the user update functionality has been implemented.![Tests](tests.png)Typically in an API project, I write three types of tests as shown above:<br>
        * Integration test - which is to do an end to end test of an endpoint by stubbing out external dependencies like the *UserStore*.
        * Tests for the controller action - to ensure that the right response codes are returned in each scenario.
        * Testing the business logic associated with an action.

## CDN cache purge

* The solution was designed assuming that a CDN service will be proxying requests to the API.
* This provides two benefits:
  * Lower resource usage and hence lower cost of running the API
  * Faster API response time for the end user
* The CDN cache is controlled by setting the appropriate cache-control headers.
* The CDN also provides an API to purge its cache records.
* When a user is updated or deleted, we call the purge endpoints to remove stale data.

## Paginating through user records

To make better use of the CDN cache, I have structured the user pagination response to only return the user ids.

```json
{
  "userIds": [
    "08b08cdc-ce50-4d69-88bd-872c16675f49",
    "c50909cf-0cf0-44df-ab05-bc54799e7ea5",
    "09ce5849-e579-42fd-ac40-0a8c1642ac46",
    "960eb7ba-5376-43c0-9248-5fbf036af453",
    "9ab98988-f73f-4c92-8092-4280b56ee6ce",
    "6963bd32-9ee3-403a-906d-6ceffd57358a",
    "11b507ec-6c92-4c3e-bc53-47306d00b53f",
    "c4344597-32cf-40a6-9c8e-1ffc1e92da3b",
    "44af4401-ae89-41ca-83bb-5dc32fe69af4",
    "f9b43f46-4585-4714-b650-d6c86a66f0ee",
    "5ce647f7-7f14-45c7-8398-f3e085838eaf",
    "95f2adaa-51a5-4b07-b745-5fb0c00db148",
    "c679cb6d-7d16-4599-950f-21560b316ecd"
  ],
  "nextPage": null,
  "previousPage": null
}
```

The key benefit with this approach is that **id** of a user record never changes. So this response for a page can be cached by a CDN, even when the individual user records are being updated.
The obvious downside with this approach is that the client then needs to make multiple requests to fetch each user record.

- With HTTP/1.1 browsers placed a limit on the number of concurrent requests that can be made to a domain. However, this problem is solved with the introduction of HTTP/2 which doesn�t have constraints on the number of concurrent requests.
- The individual user records will be cached at the CDN level and request to fetch the user records in a page will be made in parallel. This will ensure that the overall response time remains small.
- We can take this one step further by leveraging HTTP push. CDN services like Fastly and Cloudflare allows us to inspect the response coming from an origin server. We could then push these individual user records to a client before waiting for a request from the client.

I would like to reiterate that this was designed to cater for a high-volume workload. If that is not required, then we could go down the traditional route of returning user details along with the userId in each page.

## User record deletion

When a user record is deleted, we need to purge every page containing that *userId* in its response.
To achieve this, we rely on the surrogate-key feature of the CDN. This is explained in detail [here](https://www.fastly.com/blog/surrogate-keys-part-1).
