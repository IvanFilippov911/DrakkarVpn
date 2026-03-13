using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Metadata;

public static class CoreAlertResolutionCatalog
{
    public static readonly CoreAlertResolutionTypeDto[] All =
    {
        new("InfraRestarted",
            "Перезапуск инфраструктуры",
            "Перезагрузили сервер/агент/процесс без смены железа или провайдера."),

        new("InfraReplaced",
            "Замена инфраструктуры",
            "Перенесли нагрузку на другой сервер/инстанс/провайдера."),

        new("InfraConfigFixed",
            "Фикс конфигурации",
            "Исправили настройки: MTU, firewall, routes, sysctl, лимиты и т.п."),

        new("InfraCapacityIncreased",
            "Увеличили ёмкость",
            "Добавили новый сервер/регион, расширили пул, убрали перегруз."),

        new("CodeBugFixed",
            "Исправили баг в коде",
            "Найдена причина в коде, внесён фикс и выкатан релиз."),

        new("CodeRolledBack",
            "Откат релиза",
            "Откатились на прошлую стабильную версию."),

        new("PeerRevoked",
            "Отключили peer",
            "Отозвали конкретный peer / конфиг / ключ пользователя."),

        new("UserWarned",
            "Предупреждение пользователю",
            "Отправили предупреждение без жёстких ограничений."),

        new("UserRestricted",
            "Ограничили пользователя",
            "Ограничили скорость/кол-во устройств/регион и т.п."),

        new("UserBanned",
            "Бан пользователя",
            "Полный запрет пользоваться сервисом."),

        new("ExternalProviderIssue",
            "Проблема у провайдера",
            "Инцидент на стороне DC/провайдера/внешнего сервиса."),

        new("FalsePositive",
            "Ложное срабатывание",
            "Алерт сработал, но реальной проблемы не было."),

        new("ThresholdAdjusted",
            "Порог скорректирован",
            "Подкрутили пороги/условия триггера, чтобы убрать шум."),

        new("AutoResolved",
            "Автоматически решено",
            "Метрики сами вернулись в норму, алерт закрыт автоматически.")
    };
}