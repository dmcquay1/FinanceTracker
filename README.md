FinanceTracker is the first solution\project I am creating for my portfolio.  It is an API to allow tracking of personal finances. It will be a work in progress adding functionality as I am able.  I eventually plan on another project to utilize this API, likely using React.

# May 2025

1. Created the intial API project in Visual Studio 2022.
2. Decided to incorporate Entity Framework Core for a pre-existing database.
3. Decided to use Grok to provide AI assistance for the project.  This is my first time utilizing AI.  Previously, I would have received assistance from StackOverflow or just a general Google search.
4. Had some difficult connecting to the database due to certificate/trust issues.  Grok was able to provide solutions for me to try.  Eventually got the connection to work.
5. Ran into some difficulty with scaffolding the controllers and decided to consult with Grok to provide some assistance.  I  received some recommendations and was able to incorporate the changes needed to successfully scaffold the controller.
6. Focused on the CategoriesController as the first complete controller in the app.
7. Guided Grok to provide a sample recursive algorithm to create a calculated field showing the hierarchy for each category.  I was able to leverage the algorithm to create this functionality.
8. Created a test project and began configuring to use the in Entityframeworkcore.InMemory nuget packge for my unit testing. I was attempting to create a test project where all tests could be ran at once and realized that I cannot ensure when a test is ran.  Did some refactoring to attempt to help but had
