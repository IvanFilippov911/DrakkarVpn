Цель

Users — это страница управления и мониторинга пользовательской базы Drakkar Network.

Страница нужна для:
	•	просмотра списка пользователей
	•	быстрого понимания состояния пользовательской базы
	•	просмотра ключевых user-level метрик
	•	применения доступных административных действий над пользователями
	•	выполнения массовых действий над выбранными пользователями

Главный фокус страницы — рабочая таблица пользователей. Верхние summary cards являются вспомогательным обзорным блоком, но не должны доминировать над таблицей.

⸻

Основной сценарий
	1.	Администратор открывает страницу /users.
	2.	Видит короткую summary-полосу из 4 карточек:
	•	всего пользователей
	•	активные подписки
	•	истекают в ближайшие 3 дня
	•	online сейчас
	3.	Ниже видит таблицу пользователей с фильтрами, сортировкой и пагинацией.
	4.	Может:
	•	отфильтровать список по уже существующим backend-фильтрам
	•	отсортировать список по уже существующим backend sort-полям
	•	перейти между страницами списка
	•	выбрать пользователей чекбоксами
	•	применить доступные массовые действия через кнопку Update
	•	выполнить доступные row actions
	5.	Данные автоматически обновляются раз в 20 секунд.
	6.	Если summary загрузилось, а таблица нет — summary остаётся видимым, а таблица уходит в error state.
	7.	Если отдельные значения недоступны, в ячейках отображается —.

⸻

Источники данных

Users list

Endpoint:

GET /api/admin/users

Request query parameters:
	•	search: string?
	•	status: UserStatus?
	•	subscriptionStatus: SubscriptionStatus?
	•	sortBy: UsersSortBy
	•	userSortDirection: UserSortDirection
	•	page: int
	•	pageSize: int

Response contract:

PagedResponseDto<AdminUserCardDto>

Поля элемента:
	•	user.id: Guid
	•	user.telegram: long?
	•	user.createdAtUtc: DateTime
	•	user.status: UserStatus
	•	user.isOnline: bool
	•	user.deviceCount: int
	•	user.lastSeenUtc: DateTime?
	•	subscription.isActive: bool
	•	subscription.endAtUtc: DateTime
	•	subscription.maxDevices: int
	•	trafficLast24hBytes: double?

Назначение:
основной источник таблицы пользователей.

Примечание:
	•	если subscription.endAtUtc равен DateTime.UnixEpoch, frontend должен трактовать это как отсутствие данных и отображать —

⸻

User details

Endpoint:

GET /api/admin/users/{userId}/details

Response contract:

AdminUserDetailsDto

Поля:
	•	user: AdminUserSummaryDetailDto
	•	realtime: AdminUserRealtimeDto
	•	devices: UserDeviceShortDto[]
	•	alerts: AdminUserAlertDto[]

Назначение:
источник будущей деталки пользователя.

Примечание:
	•	в MVP деталка не является обязательной частью страницы
	•	поля details endpoint не должны использоваться для додумывания колонок таблицы

⸻

Ban user

Single endpoint:

POST /api/admin/users/{userId}/ban

Request contract:

BanUserRequest

Поля:
	•	reason: string?

Response:
	•	204 No Content
	•	возможен 404 Not Found
	•	возможен 409 Conflict

Bulk endpoint:

POST /api/admin/users/ban

Request contract:

BulkBanUsersRequest

Поля:
	•	userIds: Guid[]
	•	reason: string?

Response contract:

BulkUsersOperationResponse

Поля:
	•	succeededUserIds: Guid[]
	•	notFoundUserIds: Guid[]
	•	failedUserIds: Guid[]
	•	failureDetails: BulkUserFailureDetail[]?

Назначение:
бан одного или нескольких пользователей.

⸻

Unban user

Single endpoint:

POST /api/admin/users/{userId}/unban

Response:
	•	204 No Content

Bulk endpoint:

POST /api/admin/users/unban

Request contract:

BulkUnbanUsersRequest

Поля:
	•	userIds: Guid[]

Response contract:

BulkUsersOperationResponse

Назначение:
разбан одного или нескольких пользователей.

⸻

Mark user internal

Single endpoint:

POST /api/admin/users/{userId}/internal

Request contract:

MarkUserInternalRequest

Поля:
	•	isInternal: bool

Response:
	•	204 No Content
	•	возможен 404 Not Found

Bulk endpoint:

POST /api/admin/users/internal

Request contract:

BulkMarkUsersInternalRequest

Поля:
	•	userIds: Guid[]
	•	isInternal: bool

Response contract:

BulkUsersOperationResponse

Назначение:
установка internal-статуса для одного или нескольких пользователей.

Примечание:
	•	isInternal отсутствует в list contract, поэтому текущее internal-состояние не может быть показано в таблице

⸻

Grant subscription

Single endpoint:

POST /api/admin/users/{userId}/subscriptions/grant

Request contract:

AdminGrantSubscriptionRequest

Поля:
	•	tariffId: Guid
	•	deviceCount: int?

Response contract:

SubscriptionDto

Bulk endpoint:

POST /api/admin/users/subscriptions/grant

Request contract:

AdminBulkGrantSubscriptionsRequest

Поля:
	•	userIds: Guid[]
	•	tariffId: Guid
	•	deviceCount: int?

Response contract:

BulkGrantSubscriptionsResponse

Поля:
	•	succeeded: Guid[]
	•	notFound: Guid[]
	•	failed: Guid[]
	•	failureDetails: BulkUserGrantFailureDetail[]?

Назначение:
выдача подписки одному или нескольким пользователям.

⸻

Cancel subscription

Single endpoint:

POST /api/admin/subscriptions/{subscriptionId}/cancel

Response:
	•	204 No Content

Bulk endpoint:

POST /api/admin/subscriptions/cancel

Request contract:

BulkCancelSubscriptionsRequest

Поля:
	•	subscriptionIds: Guid[]

Response contract:

BulkCancelSubscriptionsResponse

Поля:
	•	succeeded: Guid[]
	•	failed: Guid[]
	•	failureDetails: BulkSubscriptionFailureDetail[]?

Назначение:
аннулирование подписок.

Примечание:
	•	users list не возвращает subscriptionId
	•	массовое аннулирование подписки по выбранным пользователям напрямую backend’ом не поддерживается

⸻

Summary cards source

Для верхних summary cards используется уже существующий overview endpoint:

GET /api/admin/overview/users

Response contract:

UsersOverviewDto

Используемые поля:
	•	totalUsers: int
	•	activeSubscriptions: int
	•	expiringSoonDays3: int
	•	onlineUsersNow: int

Назначение:
источник summary cards страницы Users.

⸻

Layout

Страница состоит из следующих блоков.

1. Page header

Содержит:
	•	title: Users
	•	subtitle: краткое описание пользовательской базы и административных действий

2. Summary row

Одна строка из 4 компактных карточек:
	•	Всего пользователей
	•	Активные подписки
	•	Истекают в ближайшие 3 дня
	•	Online сейчас

Карточки нужны для быстрого понимания состояния пользовательской базы, но должны оставаться вторичными по отношению к таблице.

3. Filters row

Над таблицей располагается блок фильтров и сортировки:
	•	search
	•	status
	•	subscriptionStatus
	•	sortBy
	•	direction

4. Table section

Ниже summary cards и filters row располагается основная таблица пользователей.

5. Bulk actions bar

Над таблицей или в верхней части table section располагается bulk actions bar:
	•	checkbox selection
	•	кнопка Update
	•	выбор bulk action

6. Pagination

Под таблицей располагается пагинация.

⸻

Filters

Для MVP используются только те фильтры, которые уже поддерживает backend.

Поддерживаемые фильтры:
	•	search
	•	status
	•	subscriptionStatus

Дополнительно:
	•	фильтра по internal в MVP нет
	•	фильтра по online в MVP нет
	•	фильтра по deviceCount в MVP нет
	•	фильтра по lastSeen в MVP нет

Если backend-фильтры уже существуют, frontend должен использовать именно их и не вводить новую логику.

⸻

Sorting

Для MVP используются только те сортировки, которые уже поддерживает backend.

Поддерживаемые sort fields:
	•	CreatedAt
	•	LastSeen
	•	SubscriptionEnd
	•	Devices
	•	Traffic24h

Поддерживаемые directions:
	•	Asc
	•	Desc

Если backend поддерживает сортировку, frontend должен использовать именно её и не вводить локальную сортировку поверх списка.

⸻

Content

Summary row

Карточка “Total Users”
Поля:
	•	totalUsers: int

UI:
	•	отображать как одно основное числовое значение

⸻

Карточка “Active Subscriptions”
Поля:
	•	activeSubscriptions: int

UI:
	•	отображать как одно основное числовое значение

⸻

Карточка “Expiring Soon”
Поля:
	•	expiringSoonDays3: int

UI:
	•	отображать как одно основное числовое значение
	•	подпись должна явно указывать окно 3 days

⸻

Карточка “Online Now”
Поля:
	•	onlineUsersNow: int

UI:
	•	отображать как одно основное числовое значение

⸻

Таблица пользователей

Колонки:
	•	Select
	•	User ID
	•	Telegram
	•	Created
	•	Status
	•	Online
	•	Devices
	•	Last Seen
	•	Subscription Active
	•	Subscription End
	•	Max Devices
	•	Traffic 24h
	•	Actions
	•	Detail

Mapping колонок
	•	User ID → user.id: Guid
	•	Telegram → user.telegram: long?
	•	Created → user.createdAtUtc: DateTime
	•	Status → user.status: UserStatus
	•	Online → user.isOnline: bool
	•	Devices → user.deviceCount: int
	•	Last Seen → user.lastSeenUtc: DateTime?
	•	Subscription Active → subscription.isActive: bool
	•	Subscription End → subscription.endAtUtc: DateTime
	•	Max Devices → subscription.maxDevices: int
	•	Traffic 24h → trafficLast24hBytes: double?

UI rules
	•	user.telegram: long? при отсутствии показывать —
	•	user.lastSeenUtc: DateTime? при отсутствии показывать —
	•	subscription.endAtUtc при значении UnixEpoch показывать —
	•	trafficLast24hBytes: double? при отсутствии показывать —
	•	user.status отображать визуальным badge / status-indicator
	•	user.isOnline: bool отображать визуальным status-indicator / badge
	•	subscription.isActive: bool отображать визуальным status-indicator / badge
	•	trafficLast24hBytes: double? форматировать human-readable
	•	createdAtUtc, lastSeenUtc, subscription.endAtUtc отображать в едином date-time формате

⸻

Actions

Page-level actions

Update
Кнопка Update обязательна при наличии выбранных пользователей.

Назначение:
открыть flow применения массового действия к выбранным пользователям.

Frontend должен использовать существующие backend contracts bulk actions.

⸻

Row actions

Для MVP доступны только реальные backend actions:

Ban User
Действие:
POST /api/admin/users/{userId}/ban

Назначение:
забанить пользователя.

UI правило:
	•	destructive action
	•	confirm dialog обязателен
	•	допустимо поле reason

⸻

Unban User
Действие:
POST /api/admin/users/{userId}/unban

Назначение:
разбанить пользователя.

⸻

Grant Subscription
Действие:
POST /api/admin/users/{userId}/subscriptions/grant

Назначение:
выдать подписку пользователю.

UI правило:
	•	требуется форма с tariffId
	•	deviceCount optional

⸻

Mark Internal
Действие:
POST /api/admin/users/{userId}/internal

Назначение:
установить internal-статус пользователю.

Примечание:
	•	текущее internal-состояние не видно в list contract

⸻

Future row actions

Detail
Кнопка Detail может присутствовать визуально.

Назначение:
будущий переход к деталке пользователя.

UI правило:
	•	если detail route не реализован, кнопка не должна вести в несуществующую функциональность
	•	fake navigation запрещена

⸻

Не входят в MVP row actions

Пока не реализуются:
	•	cancel subscription по userId
	•	reset devices
	•	edit profile
	•	inline edit
	•	device-level actions из users table
	•	alert resolution actions из users table

Если на backend нет соответствующего endpoint’а, frontend не должен придумывать action.

⸻

Bulk actions

Для MVP доступны только реальные backend bulk actions.

Ban

Действие:
POST /api/admin/users/ban

Назначение:
массовый бан выбранных пользователей.

⸻

Unban

Действие:
POST /api/admin/users/unban

Назначение:
массовый разбан выбранных пользователей.

⸻

Grant Subscription

Действие:
POST /api/admin/users/subscriptions/grant

Назначение:
массовая выдача подписки выбранным пользователям.

UI правило:
	•	требуется форма с tariffId
	•	deviceCount optional

⸻

Mark Internal

Действие:
POST /api/admin/users/internal

Назначение:
массовая установка internal-статуса.

Примечание:
	•	таблица не показывает текущее значение isInternal

⸻

Не входят в MVP bulk actions

Cancel Subscription
Не включается в MVP bulk actions страницы Users.

Причина:
	•	bulk cancel существует только по subscriptionIds
	•	users list не возвращает subscriptionId
	•	прямой backend orchestration по выбранным userIds отсутствует

Reset Devices
Не включается в MVP bulk actions страницы Users.

Причина:
	•	endpoint/controller wiring не найден

⸻

Pagination

Пагинация обязательна.

Источник:
PagedResponseDto<AdminUserCardDto>

Поля:
	•	page: int
	•	pageSize: int
	•	total: int
	•	totalPages: int

Тип:
page-based pagination.

⸻

Состояния

Loading

Минимум два независимых loading state:
	•	summary cards
	•	table

Partial Error

Если summary cards загрузились, а таблица нет:
	•	cards остаются видимыми
	•	таблица отображается в error state

Если таблица загрузилась, а summary cards нет:
	•	таблица остаётся видимой
	•	проблемная карточка или блок отображается как error card

Missing Data

Если отдельное значение недоступно:
	•	отображать —

Это особенно относится к:
	•	user.telegram
	•	user.lastSeenUtc
	•	trafficLast24hBytes
	•	subscription.endAtUtc == UnixEpoch

Success

При успешной загрузке отображаются:
	•	4 summary cards
	•	filters row
	•	bulk actions bar
	•	таблица пользователей
	•	пагинация

⸻

Ограничения MVP

В MVP не реализуется:
	•	обязательная деталка пользователя
	•	cancel subscription по выбранным пользователям
	•	reset devices
	•	фильтры, которых нет в backend contract
	•	сортировки, которых нет в backend contract
	•	inline editing
	•	bulk actions без backend support
	•	device-level management из users list
	•	alert-level management из users list

Также в MVP не требуется выводить:
	•	поля, которых нет в текущем list contract
	•	detail-only поля в таблице
	•	дополнительные служебные метаданные пользователя
	•	future actions без backend support

⸻

Навигация

Страница доступна по маршруту:

/users

Переход на страницу выполняется через top bar.

Деталка пользователя в MVP не реализуется как обязательный экран, поэтому:
	•	фальшивая навигация в пустую route не рекомендуется
	•	кнопка Detail не должна вести в несуществующую функциональность