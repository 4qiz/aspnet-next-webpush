"use client";

import { registerServiceWorker } from "@/lib/registerServiceWorker";
import { useEffect, useState } from "react";

export const dynamic = "force-dynamic";

export default function Home() {
  const [isSubscribed, setIsSubscribed] = useState(false);

  useEffect(() => {
    async function setupPushNotifications() {
      await registerServiceWorker();
      if ("Notification" in window && Notification.permission !== "granted") {
        const permission = await Notification.requestPermission();
        if (permission === "granted") {
          subscribeToPush();
        }
      }
    }

    setupPushNotifications();
  }, []);

  async function subscribeToPush() {
    const registration = await navigator.serviceWorker.ready;
    const existingSubscription =
      await registration.pushManager.getSubscription();

    if (existingSubscription) {
      console.log("4-------", existingSubscription);

      setIsSubscribed(true);
      return;
    }

    const vapidPublicKey = process.env.NEXT_PUBLIC_VAPID_PUBLIC_KEY!;
    const newSubscription = await registration.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(vapidPublicKey),
    });

    const res = await fetch("http://localhost:3000/api/push", {
      method: "POST",
      body: JSON.stringify(newSubscription),
      headers: { "Content-Type": "application/json" },
    });

    if (res.ok) {
      setIsSubscribed(true);
    }
  }

  function urlBase64ToUint8Array(base64String: string) {
    const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
    const base64 = (base64String + padding)
      .replace(/\-/g, "+")
      .replace(/_/g, "/");
    const rawData = window.atob(base64);
    return new Uint8Array([...rawData].map((char) => char.charCodeAt(0)));
  }
  return (
    <div>
      <h1>Web Push Notifications</h1>
      <button onClick={subscribeToPush} disabled={isSubscribed}>
        {isSubscribed ? "Subscribed ✅" : "Subscribe to Notifications"}
      </button>
    </div>
  );
}
