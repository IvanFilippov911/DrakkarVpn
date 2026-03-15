# Admin Auth System Map

## 1. MODULE STRUCTURE

### Application
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/IAdminAuthUnitOfWork.cs` — `IAdminAuthUnitOfWork` — контракт сохранения изменений admin auth.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/IAdminAuthCommand.cs` — `IAdminAuthCommand<TResponse>`, `IAdminAuthCommand` — маркеры команд, которые должны идти через auth unit-of-work pipeline.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Repositories/IAdminRefreshTokenRepository.cs` — `IAdminRefreshTokenRepository` — контракт доступа к refresh token storage.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminAuthService.cs` — `IAdminAuthService` — фасадный контракт login/refresh/logout/logout-all/current-admin.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminAuthenticationService.cs` — `IAdminAuthenticationService` — контракт проверки admin credentials и загрузки активного admin.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminCurrentProfileService.cs` — `IAdminCurrentProfileService` — контракт выдачи текущего admin profile.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminJwtTokenService.cs` — `IAdminJwtTokenService` — контракт выпуска access JWT.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminProfileFactory.cs` — `IAdminProfileFactory` — контракт сборки admin profile из ролей.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/IAdminRefreshSessionService.cs` — `IAdminRefreshSessionService` — контракт создания, ротации и отзыва refresh session.
- `modules/DrakkarVpn.AdminAuth/Application/Abstractions/Services/ICurrentAdminAccessor.cs` — `ICurrentAdminAccessor` — контракт чтения текущего admin из claims/HTTP context.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/AdminAuthService.cs` — `AdminAuthService` — оркестратор auth flow: authentication, profile, JWT, refresh-session, current admin.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/AdminAuthenticationService.cs` — `AdminAuthenticationService` — проверяет email/password через `UserManager`, lockout, активность, обновляет `LastLoginAtUtc`.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/AdminCurrentProfileService.cs` — `AdminCurrentProfileService` — получает текущий профиль через `ICurrentAdminAccessor`.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/AdminProfileFactory.cs` — `AdminProfileFactory` — превращает admin id/email/roles в `AdminCurrentAdminProfile` с вычисленными permissions.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/AdminRefreshSessionService.cs` — `AdminRefreshSessionService` — core-логика refresh token: создание plaintext token, SHA-256 hash, rotation, revoke, revoke-all.
- `modules/DrakkarVpn.AdminAuth/Application/Authentication/CurrentAdminAccessor.cs` — `CurrentAdminAccessor` — читает `sub`, `email`, `role`, `permission` claims и собирает `AdminCurrentAdminProfile`.
- `modules/DrakkarVpn.AdminAuth/Application/Exceptions/AdminInactiveException.cs` — `AdminInactiveException` — ошибка неактивного admin.
- `modules/DrakkarVpn.AdminAuth/Application/Exceptions/InvalidAdminCredentialsException.cs` — `InvalidAdminCredentialsException` — ошибка неверной admin authentication.
- `modules/DrakkarVpn.AdminAuth/Application/Exceptions/InvalidRefreshTokenException.cs` — `InvalidRefreshTokenException` — ошибка невалидной refresh session.
- `modules/DrakkarVpn.AdminAuth/Application/Exceptions/MissingRefreshTokenException.cs` — `MissingRefreshTokenException` — ошибка отсутствующей refresh cookie/session.

### Contracts / DTOs
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Requests/AdminJwtTokenDescriptor.cs` — `AdminJwtTokenDescriptor` — DTO admin identity для выпуска JWT/claims principal.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Requests/AdminLoginRequest.cs` — `AdminLoginRequest` — DTO login-запроса модуля.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Requests/AdminLogoutSessionRequest.cs` — `AdminLogoutSessionRequest` — DTO logout одной refresh session.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Requests/AdminRefreshSessionRequest.cs` — `AdminRefreshSessionRequest` — DTO refresh-запроса с IP/User-Agent.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminAccessToken.cs` — `AdminAccessToken` — DTO access token value, expiry, `jti`.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminCurrentAdminProfile.cs` — `AdminCurrentAdminProfile` — DTO текущего admin с ролями и permissions.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminIssuedRefreshSession.cs` — `AdminIssuedRefreshSession` — DTO созданной/ротированной refresh session.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminLoginResult.cs` — `AdminLoginResult` — DTO результата login.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminRefreshSessionResult.cs` — `AdminRefreshSessionResult` — DTO результата refresh.
- `modules/DrakkarVpn.AdminAuth/Application/Contracts/Responses/AdminSessionTokens.cs` — `AdminSessionTokens` — DTO пары access/refresh токенов.

### Authorization
- `modules/DrakkarVpn.AdminAuth/Application/Authorization/AdminClaimTypes.cs` — `AdminClaimTypes` — константа custom claim `permission`.
- `modules/DrakkarVpn.AdminAuth/Application/Authorization/AdminPermissions.cs` — `AdminPermissions` — полный список auth permissions.
- `modules/DrakkarVpn.AdminAuth/Application/Authorization/AdminPolicies.cs` — `AdminPolicies` — policy names, привязанные к permission constants.
- `modules/DrakkarVpn.AdminAuth/Application/Authorization/AdminRolePermissions.cs` — `AdminRolePermissions` — маппинг ролей в permissions.
- `modules/DrakkarVpn.AdminAuth/Application/Authorization/AdminRoles.cs` — `AdminRoles` — список ролей `SuperAdmin`, `Operator`, `ReadOnly`.

### Options / configuration
- `modules/DrakkarVpn.AdminAuth/Application/Options/AdminBootstrapOptions.cs` — `AdminBootstrapOptions` — настройки bootstrap admin email/password.
- `modules/DrakkarVpn.AdminAuth/Application/Options/AdminJwtOptions.cs` — `AdminJwtOptions` — настройки signing key, issuer, audience, access token TTL.
- `modules/DrakkarVpn.AdminAuth/Application/Options/AdminLockoutOptions.cs` — `AdminLockoutOptions` — настройки identity lockout.
- `modules/DrakkarVpn.AdminAuth/Application/Options/AdminRefreshTokenOptions.cs` — `AdminRefreshTokenOptions` — lifetime refresh token.

### Infrastructure
- `modules/DrakkarVpn.AdminAuth/Infrastructure/Auth/AdminClaimsPrincipalFactory.cs` — `AdminClaimsPrincipalFactory` — строит `ClaimsPrincipal` из `AdminJwtTokenDescriptor`, ролей и permissions.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/Bootstrap/AdminAuthBootstrapSeeder.cs` — `AdminAuthBootstrapSeeder` — создает роли и bootstrap admin с ролью `SuperAdmin`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/Jwt/AdminJwtTokenService.cs` — `AdminJwtTokenService` — подписывает и выпускает access JWT с `sub`, `email`, `jti`, role, permission claims.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthDbContext.cs` — `AdminAuthDbContext` — `IdentityDbContext` для схемы `admin_auth` и `DbSet<AdminRefreshToken>`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthDbContextFactory.cs` — `AdminAuthDbContextFactory` — design-time factory для EF tooling.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthUnitOfWork.cs` — `AdminAuthUnitOfWork` — реализация `IAdminAuthUnitOfWork` поверх `SaveChangesAsync`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityInfrastructureModelBuilderExtensions.cs` — `AdminIdentityInfrastructureModelBuilderExtensions` — переносит стандартные identity tables в `admin_*`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityRoleConfiguration.cs` — `AdminIdentityRoleConfiguration` — EF mapping `admin_roles`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityUserConfiguration.cs` — `AdminIdentityUserConfiguration` — EF mapping `admin_users`, индексы по email и is_active.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminRefreshTokenConfiguration.cs` — `AdminRefreshTokenConfiguration` — EF mapping `admin_refresh_tokens`, FK, unique/indexes.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/AdminRefreshToken.cs` — `AdminRefreshToken` — refresh-session entity с hash, expiry, revoke/rotation metadata, IP/User-Agent.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/Identity/AdminIdentityRole.cs` — `AdminIdentityRole` — identity role entity.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/Identity/AdminIdentityUser.cs` — `AdminIdentityUser` — identity user entity с `IsActive`, `CreatedAtUtc`, `LastLoginAtUtc`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Repositories/AdminRefreshTokenRepository.cs` — `AdminRefreshTokenRepository` — repository refresh session, включая `FOR UPDATE` при rotation.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/20260314133808_Initial.cs` — `Initial` — initial migration схемы `admin_auth`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/20260314133808_Initial.Designer.cs` — `Initial` — auto-generated migration model.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/AdminAuthDbContextModelSnapshot.cs` — `AdminAuthDbContextModelSnapshot` — snapshot EF-модели auth schema.

### DI / entry
- `modules/DrakkarVpn.AdminAuth/DI/AdminAuthAdminHostServiceCollectionExtensions.cs` — `AdminAuthAdminHostServiceCollectionExtensions` — host-side auth wiring: JWT infra, bearer auth, policies, refresh options, bootstrap options, auth services, `HttpContextAccessor`.
- `modules/DrakkarVpn.AdminAuth/DI/AdminAuthBootstrapExtensions.cs` — `AdminAuthBootstrapExtensions` — extension `SeedAdminAuthAsync()`.
- `modules/DrakkarVpn.AdminAuth/DI/AdminAuthInfrastructureServiceCollectionExtensions.cs` — `AdminAuthInfrastructureServiceCollectionExtensions` — wiring `AdminAuthDbContext`, `IdentityCore`, roles, EF stores, refresh token repository.
- `modules/DrakkarVpn.AdminAuth/DI/AdminAuthJwtServiceCollectionExtensions.cs` — `AdminAuthJwtServiceCollectionExtensions` — bind/validate `AdminJwtOptions`, register `IAdminJwtTokenService`.
- `modules/DrakkarVpn.AdminAuth/Entry.cs` — `Entry` — публичные extension points `AddAdminAuthInfrastructure()` и `AddAdminAuthAdminHost()`.
- `modules/DrakkarVpn.AdminAuth/DrakkarVpn.AdminAuth.csproj` — типов нет — сборка и package/project dependencies модуля.

## 2. ADMIN HOST INTEGRATION

### Wiring / composition
- `hosts/DrakkarVpn.Admin.Api/Program.cs` — top-level statements — зачем: собирает host, включает `UseAuthentication()`, `UseAuthorization()`, `UseExceptionHandling()`, вызывает `SeedAdminAuthAsync()`. Связь: запускает весь auth pipeline и bootstrap модуля.
- `hosts/DrakkarVpn.Admin.Api/DI/AdminHostCompositionExtensions.cs` — `AdminHostCompositionExtensions` — зачем: собирает инфраструктуру и host modules. Связь: вызывает `AddAdminAuthInfrastructure(configuration)` и `AddAdminAuthAdminHost(configuration)`.
- `hosts/DrakkarVpn.Admin.Api/DI/HostServiceCollectionExtensions.cs` — `HostServiceCollectionExtensions` — зачем: регистрирует host API, `IAdminAuthCookieService`, `IAdminAuthRequestContextAccessor`, swagger bearer scheme. Связь: добавляет host-side адаптеры вокруг модульного auth.
- `hosts/DrakkarVpn.Admin.Api/DI/MediatRExtensions.cs` — `MediatRExtensions` — зачем: поднимает MediatR handlers хоста. Связь: через него активируются auth commands/handlers, которые вызывают `IAdminAuthService`.
- `hosts/DrakkarVpn.Admin.Api/DrakkarVpn.Admin.Api.csproj` — типов нет — зачем: связывает проект с нужными сборками. Связь: содержит `ProjectReference` на `modules/DrakkarVpn.AdminAuth`.

### Host-side auth adapters
- `hosts/DrakkarVpn.Admin.Api/Application/Abstractions/IAdminAuthCookieService.cs` — `IAdminAuthCookieService` — зачем: контракт работы с refresh cookie. Связь: контроллер получает refresh token и передает его в модуль.
- `hosts/DrakkarVpn.Admin.Api/Application/Abstractions/IAdminAuthRequestContextAccessor.cs` — `IAdminAuthRequestContextAccessor` — зачем: контракт получения IP/User-Agent. Связь: эти значения прокидываются в login/refresh requests модуля.
- `hosts/DrakkarVpn.Admin.Api/API/Auth/AdminAuthCookieService.cs` — `AdminAuthCookieService` — зачем: ставит, читает, удаляет cookie `drakkar_admin_refresh_token`. Связь: обслуживает refresh/logout вокруг `IAdminAuthService`; бросает `MissingRefreshTokenException` модуля.
- `hosts/DrakkarVpn.Admin.Api/API/Auth/AdminAuthRequestContextAccessor.cs` — `AdminAuthRequestContextAccessor` — зачем: читает IP/User-Agent из `HttpContext`. Связь: формирует request metadata для auth flows модуля.

### Auth API contracts / mapping
- `hosts/DrakkarVpn.Admin.Api/API/Contracts/Auth/Requests/AdminLoginApiRequest.cs` — `AdminLoginApiRequest` — зачем: входной HTTP body логина. Связь: превращается в `LoginAdminCommand`, далее в `AdminLoginRequest`.
- `hosts/DrakkarVpn.Admin.Api/API/Contracts/Auth/Responses/AdminAuthApiResponse.cs` — `AdminAuthApiResponse` — зачем: HTTP response для `login` и `refresh`. Связь: строится из `AdminLoginResult` / `AdminRefreshSessionResult`.
- `hosts/DrakkarVpn.Admin.Api/API/Contracts/Auth/Responses/AdminCurrentAdminApiResponse.cs` — `AdminCurrentAdminApiResponse` — зачем: HTTP response для `me`. Связь: строится из `AdminCurrentAdminProfile`.
- `hosts/DrakkarVpn.Admin.Api/API/Mappings/AdminAuthApiMapping.cs` — `AdminAuthApiMapping` — зачем: переводит модульные auth DTO в API DTO. Связь: прямой mapping из auth-контрактов модуля.

### Commands / handlers / query
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LoginAdmin/LoginAdminCommand.cs` — `LoginAdminCommand` — зачем: MediatR-команда логина. Связь: реализует `IAdminAuthCommand<AdminLoginResult>`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LoginAdmin/LoginAdminHandler.cs` — `LoginAdminHandler` — зачем: адаптер login-команды к модулю. Связь: вызывает `IAdminAuthService.LoginAsync(...)`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/RefreshAdminSession/RefreshAdminSessionCommand.cs` — `RefreshAdminSessionCommand` — зачем: MediatR-команда refresh flow. Связь: реализует `IAdminAuthCommand<AdminRefreshSessionResult>`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/RefreshAdminSession/RefreshAdminSessionHandler.cs` — `RefreshAdminSessionHandler` — зачем: адаптер refresh-команды к модулю. Связь: вызывает `IAdminAuthService.RefreshAsync(...)`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LogoutAdminSession/LogoutAdminSessionCommand.cs` — `LogoutAdminSessionCommand` — зачем: MediatR-команда logout одной сессии. Связь: реализует `IAdminAuthCommand<Unit>`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LogoutAdminSession/LogoutAdminSessionHandler.cs` — `LogoutAdminSessionHandler` — зачем: вызывает logout одной refresh session. Связь: вызывает `IAdminAuthService.LogoutAsync(...)`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LogoutAllAdminSessions/LogoutAllAdminSessionsCommand.cs` — `LogoutAllAdminSessionsCommand` — зачем: MediatR-команда logout-all. Связь: реализует `IAdminAuthCommand<Unit>`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Commands/Auth/LogoutAllAdminSessions/LogoutAllAdminSessionsHandler.cs` — `LogoutAllAdminSessionsHandler` — зачем: вызывает глобальный logout текущего admin. Связь: вызывает `IAdminAuthService.LogoutAllAsync(...)`.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Queries/Auth/GetCurrentAdmin/GetCurrentAdminQuery.cs` — `GetCurrentAdminQuery` — зачем: запрос текущего профиля. Связь: возвращает `AdminCurrentAdminProfile` модуля.
- `hosts/DrakkarVpn.Admin.Api/Application/Features/Queries/Auth/GetCurrentAdmin/GetCurrentAdminHandler.cs` — `GetCurrentAdminHandler` — зачем: адаптер current-admin query к модулю. Связь: вызывает `IAdminAuthService.GetCurrentAdminAsync(...)`.

### Auth controller
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` — `AdminAuthController` — зачем: HTTP entry point для `login`, `refresh`, `logout`, `logout-all`, `me`. Связь: работает через MediatR auth commands/query, host cookie service и request context accessor, а дальше уходит в модуль.

### Protected admin endpoints
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminCoreAlertsController.cs` — `AdminCoreAlertsController` — зачем: защищённый observability alerts API. Связь: использует `AdminPolicies.ObservabilityRead`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminCoreErrorsController.cs` — `AdminCoreErrorsController` — зачем: защищённый errors API. Связь: использует `AdminPolicies.ObservabilityRead`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminCoreHealthController.cs` — `AdminCoreHealthController` — зачем: защищённый health API. Связь: использует `AdminPolicies.ObservabilityRead`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminPeersController.cs` — `AdminPeersController` — зачем: защищённые peer operations. Связь: использует `AdminPolicies.PeersManage`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminServersController.cs` — `AdminServersController` — зачем: защищённые server endpoints. Связь: class-level `[Authorize]`, method-level `ServersRead`, `ObservabilityRead`, `ServersManage`, `PeersManage`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminSubscriptionsController.cs` — `AdminSubscriptionsController` — зачем: защищённые subscription operations. Связь: использует `AdminPolicies.SubscriptionsManage`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminTariffsController.cs` — `AdminTariffsController`, `CreateTariffBody`, `UpdateTariffBody` — зачем: защищённый tariffs API. Связь: использует `AdminPolicies.TariffsManage`.
- `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminUsersController.cs` — `AdminUsersController` — зачем: защищённый users API. Связь: class-level `[Authorize]`, method-level `UsersRead`, `UsersManage`.

### Settings
- `hosts/DrakkarVpn.Admin.Api/appsettings.json` — типов нет — зачем: базовые runtime settings host-а. Связь: содержит секцию `AdminJwt`.
- `hosts/DrakkarVpn.Admin.Api/appsettings.Development.json` — типов нет — зачем: dev settings host-а. Связь: содержит секции `AdminBootstrap` и `AdminJwt`.

## 3. DATABASE STRUCTURE

### Auth-related DB files
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthDbContext.cs` — `AdminAuthDbContext` — основной auth `DbContext`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthDbContextFactory.cs` — `AdminAuthDbContextFactory` — design-time factory.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/AdminAuthUnitOfWork.cs` — `AdminAuthUnitOfWork` — commit boundary для auth commands.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/Identity/AdminIdentityUser.cs` — `AdminIdentityUser` — таблица admin users.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/Identity/AdminIdentityRole.cs` — `AdminIdentityRole` — таблица admin roles.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Entities/AdminRefreshToken.cs` — `AdminRefreshToken` — таблица refresh sessions.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityUserConfiguration.cs` — `AdminIdentityUserConfiguration` — mapping `admin_users`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityRoleConfiguration.cs` — `AdminIdentityRoleConfiguration` — mapping `admin_roles`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminRefreshTokenConfiguration.cs` — `AdminRefreshTokenConfiguration` — mapping `admin_refresh_tokens`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Configurations/AdminIdentityInfrastructureModelBuilderExtensions.cs` — `AdminIdentityInfrastructureModelBuilderExtensions` — mapping identity auxiliary tables.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Repositories/AdminRefreshTokenRepository.cs` — `AdminRefreshTokenRepository` — repository над `AdminAuthDbContext`.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/20260314133808_Initial.cs` — `Initial` — миграция создания auth schema.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/20260314133808_Initial.Designer.cs` — `Initial` — generated migration metadata.
- `modules/DrakkarVpn.AdminAuth/Infrastructure/EF/Migrations/AdminAuthDbContextModelSnapshot.cs` — `AdminAuthDbContextModelSnapshot` — snapshot auth model.
- `hosts/DrakkarVpn.DbMigrator/Program.cs` — top-level statements — применяет `AdminAuthDbContext.Database.MigrateAsync()`.

### Tables created
Схема: `admin_auth`.

Таблицы:
- `admin_roles`
- `admin_users`
- `admin_role_claims`
- `admin_refresh_tokens`
- `admin_user_claims`
- `admin_user_logins`
- `admin_user_roles`
- `admin_user_tokens`

### DbContext ownership
- Все таблицы auth-схемы управляются `AdminAuthDbContext`.
- `AdminAuthDbContext` наследуется от `IdentityDbContext<AdminIdentityUser, AdminIdentityRole, Guid, ...>` и дополнительно содержит `DbSet<AdminRefreshToken> RefreshTokens`.

### Storage model
- Admin users: `admin_auth.admin_users`
- Admin roles: `admin_auth.admin_roles`
- User-role links: `admin_auth.admin_user_roles`
- Identity claims/logins/tokens: `admin_auth.admin_user_claims`, `admin_auth.admin_role_claims`, `admin_auth.admin_user_logins`, `admin_auth.admin_user_tokens`
- Refresh sessions: `admin_auth.admin_refresh_tokens`

### Refresh session storage
- В БД хранится `TokenHash`, а не plaintext refresh token.
- Hash вычисляется в `AdminRefreshSessionService` через SHA-256.
- `admin_refresh_tokens` содержит: `admin_user_id`, `token_hash`, `expires_at_utc`, `created_at_utc`, `revoked_at_utc`, `replaced_by_token_hash`, `created_by_ip`, `user_agent`.
- FK: `admin_refresh_tokens.admin_user_id -> admin_users.id`, delete behavior: `Cascade`.
- Индексы: unique `ux_admin_refresh_tokens_token_hash`, active-window index по `(admin_user_id, revoked_at_utc, expires_at_utc)`, expiry index по `expires_at_utc`.

### Identity / permissions storage
- Permissions не хранятся отдельной EF-таблицей.
- Permissions вычисляются в коде через `AdminRolePermissions`.
- Эти permissions затем попадают в `AdminCurrentAdminProfile`, `ClaimsPrincipal` и access JWT.

## 4. FLOW ENTRY POINTS

1. `POST /api/admin/auth/login`
   - Старт: `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` -> `Login`
   - Первый application entry: `LoginAdminCommand` / `LoginAdminHandler`

2. `POST /api/admin/auth/refresh`
   - Старт: `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` -> `Refresh`
   - Первый application entry: `RefreshAdminSessionCommand` / `RefreshAdminSessionHandler`

3. `POST /api/admin/auth/logout`
   - Старт: `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` -> `Logout`
   - Первый application entry: `LogoutAdminSessionCommand` / `LogoutAdminSessionHandler`

4. `POST /api/admin/auth/logout-all`
   - Старт: `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` -> `LogoutAll`
   - Первый application entry: `LogoutAllAdminSessionsCommand` / `LogoutAllAdminSessionsHandler`

5. `GET /api/admin/auth/me`
   - Старт: `hosts/DrakkarVpn.Admin.Api/API/Controllers/AdminAuthController.cs` -> `Me`
   - Первый application entry: `GetCurrentAdminQuery` / `GetCurrentAdminHandler`

6. Protected admin endpoints
   - `AdminCoreAlertsController` -> policy `AdminPolicies.ObservabilityRead`
   - `AdminCoreErrorsController` -> policy `AdminPolicies.ObservabilityRead`
   - `AdminCoreHealthController` -> policy `AdminPolicies.ObservabilityRead`
   - `AdminPeersController` -> policy `AdminPolicies.PeersManage`
   - `AdminServersController` -> class `[Authorize]`, method policies `ServersRead`, `ObservabilityRead`, `ServersManage`, `PeersManage`
   - `AdminSubscriptionsController` -> policy `AdminPolicies.SubscriptionsManage`
   - `AdminTariffsController` -> policy `AdminPolicies.TariffsManage`
   - `AdminUsersController` -> class `[Authorize]`, method policies `UsersRead`, `UsersManage`

## 5. DEPENDENCY MAP

### Login
`AdminAuthController.Login`  
→ `LoginAdminCommand`  
→ `LoginAdminHandler`  
→ `IAdminAuthService.LoginAsync`  
→ `AdminAuthService`  
→ `IAdminAuthenticationService.AuthenticateAsync`  
→ `AdminAuthenticationService`  
→ `UserManager<AdminIdentityUser>`  
→ `IAdminProfileFactory.Create`  
→ `IAdminJwtTokenService.Issue`  
→ `IAdminRefreshSessionService.CreateAsync`  
→ `AdminRefreshSessionService`  
→ `IAdminRefreshTokenRepository.AddAsync`  
→ `AdminRefreshTokenRepository`  
→ `AdminAuthDbContext`  
→ `AdminAuthUnitOfWorkBehavior`  
→ `IAdminAuthUnitOfWork.SaveChangesAsync`

### Refresh
`AdminAuthController.Refresh`  
→ `IAdminAuthCookieService.GetRequiredRefreshToken`  
→ `RefreshAdminSessionCommand`  
→ `RefreshAdminSessionHandler`  
→ `IAdminAuthService.RefreshAsync`  
→ `AdminAuthService`  
→ `IAdminRefreshSessionService.RotateAsync`  
→ `AdminRefreshSessionService`  
→ `IAdminRefreshTokenRepository.GetByTokenHashForUpdateAsync`  
→ `AdminRefreshTokenRepository`  
→ `AdminAuthDbContext` transaction  
→ replacement `AdminRefreshToken` insert  
→ `IAdminAuthenticationService.GetActiveAdminAsync`  
→ `IAdminProfileFactory.Create`  
→ `IAdminJwtTokenService.Issue`  
→ `AdminAuthUnitOfWorkBehavior`  
→ `IAdminAuthUnitOfWork.SaveChangesAsync`

### Logout
`AdminAuthController.Logout`  
→ `IAdminAuthCookieService.TryGetRefreshToken`  
→ `LogoutAdminSessionCommand`  
→ `LogoutAdminSessionHandler`  
→ `IAdminAuthService.LogoutAsync`  
→ `AdminAuthService`  
→ `IAdminRefreshSessionService.RevokeAsync`  
→ `AdminRefreshSessionService`  
→ `IAdminRefreshTokenRepository.GetByTokenHashAsync`  
→ `AdminRefreshTokenRepository`  
→ `AdminAuthDbContext`  
→ `AdminAuthUnitOfWorkBehavior`  
→ `IAdminAuthUnitOfWork.SaveChangesAsync`  
→ `IAdminAuthCookieService.DeleteRefreshToken`

### Logout all
`AdminAuthController.LogoutAll`  
→ `LogoutAllAdminSessionsCommand`  
→ `LogoutAllAdminSessionsHandler`  
→ `IAdminAuthService.LogoutAllAsync`  
→ `AdminAuthService`  
→ `IAdminCurrentProfileService.GetCurrentAsync`  
→ `AdminCurrentProfileService`  
→ `ICurrentAdminAccessor.GetRequiredCurrent`  
→ `CurrentAdminAccessor`  
→ `IAdminRefreshSessionService.RevokeAllAsync`  
→ `AdminRefreshSessionService`  
→ `IAdminRefreshTokenRepository.GetActiveByAdminUserIdAsync`  
→ `AdminRefreshTokenRepository`  
→ `AdminAuthDbContext`  
→ `AdminAuthUnitOfWorkBehavior`  
→ `IAdminAuthUnitOfWork.SaveChangesAsync`  
→ `IAdminAuthCookieService.DeleteRefreshToken`

### Current admin
Bearer JWT request  
→ `UseAuthentication()`  
→ `AddJwtBearer(... OnTokenValidated ...)` in `AdminAuthAdminHostServiceCollectionExtensions`  
→ `IAdminAuthenticationService.GetActiveAdminAsync`  
→ `AdminClaimsPrincipalFactory.Create`  
→ `UseAuthorization()`  
→ `AdminAuthController.Me`  
→ `GetCurrentAdminQuery`  
→ `GetCurrentAdminHandler`  
→ `IAdminAuthService.GetCurrentAdminAsync`  
→ `AdminAuthService`  
→ `IAdminCurrentProfileService.GetCurrentAsync`  
→ `AdminCurrentProfileService`  
→ `ICurrentAdminAccessor.GetRequiredCurrent`  
→ `CurrentAdminAccessor`

### Protected endpoints
Bearer JWT  
→ `UseAuthentication()`  
→ `OnTokenValidated` rehydrates claims via `AdminClaimsPrincipalFactory`  
→ `UseAuthorization()`  
→ policy check against `AdminPolicies.*`  
→ permission claim `AdminClaimTypes.Permission`  
→ protected controller action
