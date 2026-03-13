import { useEffect, useRef, useState } from "react";
import * as api from "./services/api";
import {
  applyTelegramTheme,
  getInitData,
  getPlatform,
  getDeviceName,
  getTelegramUserId,
  initTelegramWebApp,
  isTelegramWebApp,
} from "./services/telegram";
import { getToken, getDeviceId, setToken, setDeviceId } from "./services/storage";
import { TARIFF_ID } from "./constants";
import "./index.css";

type AppState =
  | "bootstrapping"
  | "needsPurchase"
  | "canConnect"
  | "connecting"
  | "provisioning"
  | "ready"
  | "error";

const POLL_INTERVAL_MS = 2000;

export default function App() {
  const [state, setState] = useState<AppState>("bootstrapping");
  const [errorMessage, setErrorMessage] = useState("");
  const [happLink, setHappLink] = useState("");
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  // Bootstrap: Telegram init, register, decide needsPurchase vs canConnect
  useEffect(() => {
    if (state !== "bootstrapping") return;

    if (!isTelegramWebApp()) {
      setErrorMessage("Откройте приложение через Telegram");
      setState("error");
      return;
    }

    initTelegramWebApp();
    applyTelegramTheme();
    const initData = getInitData();
    if (!initData) {
      setErrorMessage("Откройте приложение через Telegram");
      setState("error");
      return;
    }

    const telegramId = getTelegramUserId();
    if (telegramId == null) {
      setErrorMessage("Не удалось определить пользователя Telegram");
      setState("error");
      return;
    }

    (async () => {
      try {
        await api.register(telegramId);
        const token = getToken();
        if (token) {
          setState("canConnect");
        } else {
          setState("needsPurchase");
        }
      } catch (e) {
        setErrorMessage(e instanceof Error ? e.message : "Ошибка инициализации");
        setState("error");
      }
    })();
  }, [state]);

  const handlePurchase = async () => {
    const telegramId = getTelegramUserId();
    if (telegramId == null) return;
    setState("connecting");
    setErrorMessage("");
    try {
      await api.purchaseSubscription(
        telegramId,
        TARIFF_ID,
        crypto.randomUUID()
      );
      setState("canConnect");
    } catch (e) {
      setErrorMessage(e instanceof Error ? e.message : "Ошибка покупки");
      setState("error");
    }
  };

  const handleConnect = async () => {
    const initData = getInitData();
    if (!initData) {
      setErrorMessage("Нет данных Telegram");
      setState("error");
      return;
    }
    setState("connecting");
    setErrorMessage("");
    try {
      const platform = getPlatform();
      const deviceName = getDeviceName();
      const existingDeviceId = getDeviceId();
      const { token, deviceId } = await api.connectDevice(
        initData,
        deviceName,
        platform,
        existingDeviceId
      );
      setToken(token);
      setDeviceId(deviceId);

      const configRes = await api.getVpnConfig(token);
      if (configRes.status === "Ready" && configRes.happLink) {
        setHappLink(configRes.happLink);
        setState("ready");
        return;
      }
      if (configRes.status === "Pending" && configRes.jobId) {
        startProvisionPolling(configRes.jobId, token);
        setState("provisioning");
        return;
      }
      setErrorMessage("Неожиданный ответ сервера");
      setState("error");
    } catch (e) {
      setErrorMessage(e instanceof Error ? e.message : "Ошибка подключения");
      setState("error");
    }
  };

  function startProvisionPolling(jobId: string, token: string) {
    if (pollRef.current) clearInterval(pollRef.current);
    pollRef.current = setInterval(async () => {
      try {
        const job = await api.getProvisionJob(jobId, token);
        if (job.status === "Ready" && job.happLink) {
          if (pollRef.current) {
            clearInterval(pollRef.current);
            pollRef.current = null;
          }
          setHappLink(job.happLink);
          setState("ready");
          return;
        }
        if (job.status === "Failed") {
          if (pollRef.current) {
            clearInterval(pollRef.current);
            pollRef.current = null;
          }
          setErrorMessage(job.error || "Ошибка подготовки конфига");
          setState("error");
        }
      } catch (e) {
        if (pollRef.current) {
          clearInterval(pollRef.current);
          pollRef.current = null;
        }
        setErrorMessage(e instanceof Error ? e.message : "Ошибка опроса");
        setState("error");
      }
    }, POLL_INTERVAL_MS);
  }

  useEffect(() => {
    return () => {
      if (pollRef.current) {
        clearInterval(pollRef.current);
        pollRef.current = null;
      }
    };
  }, []);

  const handleRetry = () => {
    setErrorMessage("");
    const token = getToken();
    setState(token ? "canConnect" : "needsPurchase");
  };

  const handleOpenVpn = () => {
    if (happLink) window.location.href = happLink;
  };

  const titleStyle: React.CSSProperties = {
    fontSize: "1.5rem",
    fontWeight: 700,
    marginBottom: 24,
  };

  const buttonStyle: React.CSSProperties = {
    padding: "12px 24px",
    fontSize: "1rem",
    fontWeight: 600,
    borderRadius: 8,
    border: "none",
    cursor: "pointer",
    backgroundColor: "#2563eb",
    color: "#fff",
  };

  if (state === "bootstrapping") {
    return (
      <div className="app-wrap">
        <p>Инициализация...</p>
      </div>
    );
  }

  if (state === "needsPurchase") {
    return (
      <div className="app-wrap">
        <h1 style={titleStyle}>Drakkar VPN</h1>
        <button style={buttonStyle} onClick={handlePurchase}>
          Купить подписку
        </button>
      </div>
    );
  }

  if (state === "canConnect") {
    return (
      <div className="app-wrap">
        <h1 style={titleStyle}>Drakkar VPN</h1>
        <button style={buttonStyle} onClick={handleConnect}>
          Подключить VPN
        </button>
      </div>
    );
  }

  if (state === "connecting") {
    return (
      <div className="app-wrap">
        <p>Подключаем устройство...</p>
      </div>
    );
  }

  if (state === "provisioning") {
    return (
      <div className="app-wrap">
        <p>Готовим VPN-конфиг...</p>
      </div>
    );
  }

  if (state === "ready") {
    return (
      <div className="app-wrap">
        <p style={{ marginBottom: 24 }}>VPN готов к подключению</p>
        <button style={buttonStyle} onClick={handleOpenVpn}>
          Открыть VPN
        </button>
      </div>
    );
  }

  if (state === "error") {
    return (
      <div className="app-wrap">
        <p style={{ color: "#e74c3c", marginBottom: 24 }}>{errorMessage}</p>
        <button style={buttonStyle} onClick={handleRetry}>
          Попробовать снова
        </button>
      </div>
    );
  }

  return null;
}
