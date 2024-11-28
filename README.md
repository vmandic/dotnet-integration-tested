# dotnet-integration-tested

This repository contains the demo application used for a lecture: __"Building Modern and Reliable .NET Solutions Through Integration Testing"__ which will be first presented on [advtechdays.com](https://www.advtechdays.com/) Conference in Dec 2024.

# Lecture summary

Presentation link: <https://slides.com/vmandic/dotnet-integration-tested>

> In today’s fast-paced software landscape, developing reliable solutions goes beyond writing code—it’s about building a robust test suite from the start, maintaining it, and nourishing a wholesome testing culture across the team. In this demo-focused session, we will explore how to craft modern .NET solutions with confidence, leveraging integration tests to ensure stability across database interactions, external and internal services, and business logic.
>
> We will build a few complex tests for a rudimental SEO keyword rank tracking solution. You’ll see how > integration tests help simulate real-world conditions using tools like database fake data factories and HTTP > client interception to mock external APIs. The tools in the discussion will be TestServer, Docker, Moq, > `TestContainers`, and XUnit (with parallelization) emphasizing how a solid testing culture ensures long-term > reliability.
>
> By the end of this session, you’ll understand the importance of starting tests early, how to effectively > integrate databases and external services in tests, and how a well-designed testing suite can make your .NET > solutions more resilient and maintainable. This is not just a presentation—it's a live demonstration of the > power of integration tests in action.
>
> It is never too late to add tests to your codebase, and this presentation will surely encourage or at least inspire you to do so.

# Project and lecture goal

Demonstrate how to build a web based system with multiple interdependent components and storage services whilst having each of its parts unit and integration tested.
        
# .NET SDK

- `7.0.203` as set up in global.json and NuGet deps relying on it.

# How to build

`dotnet build` in git repository root to build the whole solution.

# How to test

`dotnet test` will run both unit tests and integration tests. 

# How to use

Setup the appsettings.json for required WincherAuth and WincherApi configs.

```bash
# signup
curl --location 'http://localhost:5055/signup' \
--header 'Content-Type: application/json' \
--header 'Authorization: ••••••' \
--data '{
    "Username": "test2",
    "Password": "test"
}'

# login
curl --location 'http://localhost:5065/login' \
--header 'Content-Type: application/json' \
--header 'Authorization: ••••••' \
--data '{
    "Username": "test2",
    "Password": "test"
}'

# Check if authorized
curl --location 'http://localhost:5055/check-auth' \
--header 'Authorization: Bearer eyJhbG...'

# Make SEO check
curl --location 'http://localhost:5055/seo-check' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer eyJhbG...' \
--data '{
    "Url": "http://www.advtechdays.com",
    "Keywords": ["technology"]
}'

# Read all checks
curl --location 'http://localhost:5055/seo-checks' \
 --header 'Authorization: Bearer eyJhbG...'
 
# Read a specific SEO check
curl --location 'http://localhost:5055/seo-check/67484a730f012a66a82b6e76' \
 --header 'Authorization: Bearer eyJhbG...'
```
