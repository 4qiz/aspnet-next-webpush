"use client";

import { registerServiceWorker } from "@/lib/registerServiceWorker";
import { useEffect, useState } from "react";

export const dynamic = "force-dynamic";

function urlBase64ToUint8Array(base64String: string) {
  const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding)
    .replace(/\-/g, "+")
    .replace(/_/g, "/");
  const rawData = window.atob(base64);
  return new Uint8Array([...rawData].map((char) => char.charCodeAt(0)));
}
function arrayBufferToBase64(buffer: ArrayBuffer | null) {
  if (!buffer) return null;
  const binary = String.fromCharCode(...new Uint8Array(buffer));
  return btoa(binary);
}

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
    try {
      const registration = await navigator.serviceWorker.ready;
      let subscription = await registration.pushManager.getSubscription();

      if (!subscription) {
        const vapidPublicKey = process.env.NEXT_PUBLIC_VAPID_PUBLIC_KEY;

        if (!vapidPublicKey) {
          console.log("NEXT_PUBLIC_VAPID_PUBLIC_KEY is undefined");
          throw new Error("NEXT_PUBLIC_VAPID_PUBLIC_KEY is undefined");
        }

        subscription = await registration.pushManager.subscribe({
          userVisibleOnly: true,
          applicationServerKey: urlBase64ToUint8Array(vapidPublicKey),
        });
      }

      if (!subscription) {
        console.log("subscription is null");
        return;
      }

      const subscriptionData = {
        endpoint: subscription.endpoint,
        p256dh: arrayBufferToBase64(subscription.getKey("p256dh")),
        auth: arrayBufferToBase64(subscription.getKey("auth")),
      };

      const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

      if (!apiUrl) {
        console.log("NEXT_PUBLIC_API_BASE_URL is undefined");
        throw new Error("NEXT_PUBLIC_API_BASE_URL is undefined");
      }

      const res = await fetch(`${apiUrl}/push/subscribe`, {
        method: "POST",
        body: JSON.stringify(subscriptionData),
        headers: { "Content-Type": "application/json" },
      });

      if (res.ok) {
        setIsSubscribed(true);
      }
    } catch (error) {
      console.error("Push subscription failed:", error);
    }
  }

  return (
    <div>
      <h1>Web Push Notifications</h1>
      <button
        className="p-3 border rounded-lg"
        onClick={subscribeToPush}
        disabled={isSubscribed}
      >
        {isSubscribed ? "Subscribed ✅" : "Subscribe to Notifications"}
      </button>
    </div>
  );
}
