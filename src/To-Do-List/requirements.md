You are working in an existing full-stack TodoList project.

Project context:
- Backend: ASP.NET Core Web API, EF Core, MySQL, JWT auth, FluentValidation
- Frontend: Vue 3
- Current features already exist:
  - register / login / refresh token
  - get current user
  - create task
  - create category
  - get all tasks grouped by category
  - default category is automatically created for each user
- Current codebase already has:
  - AuthController, UsersController
  - MyIdentityDbContext, TaskDbContext
  - TaskCategoryService, IdentityService
  - Vue pages for login, register, todo list, profile

Goal:
Implement a solid V1 of the app with the following features:
1. task list page polished and usable
2. create task dialog improved
3. delete task
4. edit task
5. mark task complete / incomplete
6. category management page
7. update category
8. delete category
9. API route cleanup toward resource-style endpoints where practical
10. frontend UI should look clean and consistent

Constraints:
- Do not rewrite the whole project from scratch
- Reuse existing architecture and naming where possible
- Keep backend layering clean: controllers -> services -> repositories
- Prefer interface-based DI
- Do not add unnecessary dependencies unless truly needed
- Keep changes incremental and production-minded
- Preserve current JWT auth flow
- Preserve default-category behavior when deleting categories by moving tasks to default category

Execution requirements:
- First inspect the existing codebase and summarize current structure
- Then produce a short implementation plan
- Then implement the work in small logical steps
- After each major step, explain what changed
- Run or describe the relevant validation steps
- If something in the codebase conflicts with the plan, adapt to the existing structure instead of forcing a rewrite

Definition of done:
- Backend compiles
- Frontend compiles
- User can register, login, create task, edit task, delete task, complete task, create category, edit category, delete category
- Todo list UI and dialogs are visually improved
- Profile page and navbar remain consistent with the updated design
- Existing auth flow still works

Start by analyzing the repository and listing:
1. current backend modules
2. current frontend pages/components
3. missing pieces for V1
   Then begin implementation.


Analyze this repository and create a V1 implementation plan for this TodoList app.

I want the following V1 scope:
- polished todo list page
- add task dialog improved
- edit task
- delete task
- complete/incomplete task
- category management page
- update/delete category
- keep auth flow intact
- keep current architecture style

Please do not modify code yet.
First output:
1. current project structure
2. backend modules already present
3. frontend pages/components already present
4. missing pieces
5. an ordered implementation plan

Implement the backend for V1 task and category management.

Please add:
- delete task
- update task
- complete task
- incomplete task
- get task by id if needed
- update category
- delete category

Requirements:
- keep existing EF Core structure
- keep service/repository pattern
- use interface-based DI where missing
- preserve default-category fallback behavior
- keep response structure consistent with existing project

After changes, summarize:
- added endpoints
- changed services
- changed repositories
- any DTOs/validators added

Implement the frontend V1 task management UI for this Vue app.

Please improve:
- todo list page
- add task dialog
- edit task dialog
- category management page
- navbar/profile visual consistency

Requirements:
- keep current Vue structure
- do not introduce a UI library
- use clean modern styling with consistent spacing, rounded cards, soft shadows
- support create/edit/delete/complete flows
- keep code readable and componentized

After changes, summarize:
- pages changed
- components added
- API calls added/updated

Refactor the API routes toward cleaner resource-style endpoints where practical.

Current project has legacy routes like:
- /Task/GetAllTasks
- /Task/CreateTask
- /Task/GetAllCategories

Please refactor to a cleaner style such as:
- GET /api/tasks
- POST /api/tasks
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}
- GET /api/categories
- POST /api/categories
- PUT /api/categories/{id}
- DELETE /api/categories/{id}

Requirements:
- update both backend and frontend
- avoid breaking auth routes
- keep changes minimal and coherent
- summarize all route changes clearly