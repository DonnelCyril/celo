# Questionnaire

## 1. What is your favourite design pattern, and why?

I quite like the Mediator design pattern. I have designed the API exercise using a library called MediatR that is an implementation of this pattern.
Adopting this pattern, I find that my classes are smaller and focussed and has a smaller number of dependencies than otherwise.
Typically, we go down the route of a Service class that orchestrates the interaction between different objects. Borrowing a quote from the internet - "The problem with Service classes is that they tend to be sticky". It is hard to define a boundary for a Service and determine which methods are included in a Service and which are not.
Another benefit I found with this approach is that it provides me a with a means to consistently apply cross-cutting concerns (e.g.: Logging, Caching, Metrics, Retries etc.) across the application.
While we could implement the caching or logging logic directly within the service class, I find that this creates a lot of noise and hides the core functionality of these methods.
Mediator pattern provides us with hooks to apply these cross-cutting concerns in a central place. In the MediatR library terminology, we can create what are known as behaviour classes.
They allow us to add additional functionality between when a message is published by the mediator to when it is handled by the message handler. This is similar to the middleware concept (Behaviors in MediatR terminology) in frameworks like Redux.

## 2. For your favourite programming language, tell me about a new (or upcoming) language feature that has you excited. Why is it exciting for you?

I am fan of functional programming in general. So, any new feature in this space really excites me.
The new pattern matching support in C# is cool. It is not as fully featured as is the case with a functional language like F# but is a step in the right direction. I feel this helps write code that is expressive and intent revealing.
I am also looking forward to the upcoming Records feature in C#. This helps to define types in a much more concise manner and comes with several other features baked in.
I feel there is too much friction in creating types (classes) in C# when compared to defining types in languages like F#. As a result of which we tend to keep extending existing classes rather than defining new ones.
Compare this to defining types in F#. These types represent the results of different operations that can be performed against a user. They are short concise and there is very less typing involved.

```fsharp
type UpdateUserResult =
	| SuccessfullUpdate of string
	| NewerVersionOfUserFound of string
	| UserNotFound
	| FailedToApplyUpdate of string

type GetUserResult =
	| UserFound of (string, User)
	| UserNotFound
	| UserNotModified

type CreateUserResult =
	| CreatedUser of (string, User)

type DeleteUser =
	| UserDeleted
	| UserNotFound
```

The same implementation in C# span several classes across different files.<br>
It is not unheard of that sometimes the entire domain model of an application can be contained with a single file in an F# project. While this is not a goal by any means it highlights how defining types in F# is much smaller and concise. I feel this is invaluable when reading the source code and forming a mental model of the system.
Records feature won’t solve all the above problems. Again, it a step in the right direction to encourage developers to use more types when designing solutions. I believe the Records feature is also a pre-requisite before C# will have native support for more functional goodness like Discriminated Unions.

## 3. What do you not like to see when you're reviewing your own of another colleague's code?

There are few different things that I look for when doing a code review.

* Is the coding style consistent with the rest of the application, are the right patterns and practise being followed.
* Is the new code introducing any duplications, is there a possibility for re-use, can the code be extracted into a common layer than they can be re-used in other places.
* I also believe that we read code more often than writing it, so I give high emphasis for things that can make the code more readable.
    * Smaller classes and smaller methods 
    * Does the variable / class names make sense. 
    * Code comments where appropriate.
* Is the code in the right place in the project structure.
* Are there sufficient tests?

## 4. Tell me about a time you fixed a performance issue.

### 1. Improving API performance in general

I have helped troubleshoot and fix different type of API performance issues. Most of them had to do with poor response times when operating under load. The first step is to identify the type of problem - is it a CPU bound issue, is it a memory related, or is it an issue with one of the dependent systems. I have leveraged performance counters when diagnosing locally or in test environments. For production environments, I have used APM tools like New Relic to help diagnose the issue. Have run into different types of problems here, some of the common ones being:

#### 1. Lack of caching

This was putting heavy load on SQL and other dependent systems. The solution was to use a combination of caching options - in-memory, ASP.NET output cache, HTTP cache headers in conjunction with a CDN service like Fastly / Cloudflare. I have used a CDN based caching technique for the API exercise and is described in detail in the solution.

#### 2. External dependencies not responding in a timely manner

The application that I worked on consumed several external APIs. A slow external API would increase the overall response time. Unfortunately, this also happened to be a legacy system when external API calls were invoked synchronously. A slow API would block an application thread eventually leading to a depletion of threads available to handle new requests. These issues were diagnosed either using APM tools / or by parsing request logs. I have written python scripts that can parse large log files and generating different plots that helps diagnose the issue. Have applied a variety of different solutions to help mitigate some of these issues. As an immediate measure, the first step is to determine if the system can operate without the service causing the bottleneck. If that is the case, then shut down that portion of the app through feature flags or config settings available. I have also made use of patterns like CircuitBreaker to automate this process without manual intervention. Also rewrote a large portion of the app to adopt async IO which mitigated the thread depletion issue.<br><br>
Below are some more specific cases of performance issues that I have encountered and how the troubleshooting was done.

### 2. Incorrectly registering dependencies with the IOC container

Ran into a scenario where a configuration service which was supposed to be a singleton was registered under the transient scope with the IOC container. This configuration service reads configuration from a config file. The application functioned as normal till we carried out a load test which showed the file system being a bottleneck. It was easily to spot the faulty as there were very few places that was dealing with file system. If this wasn’t the case, then I would look at using some tool that will allow me to monitor my application's file system usage. SysInternals has several tools that can assist with such kind of diagnosis.

### 3. Out of memory exceptions.

I have used the Visual studio memory profiler to identity the root cause. The issue was caused by an IEnumerable collection in a cache that was holding references to a larger collection of objects. This prevented it from being garbage collected. The solution was to enumerate and build a List of items before adding it to the cache. This removed the dependency to the source collection allowing it to be garbage collected.
