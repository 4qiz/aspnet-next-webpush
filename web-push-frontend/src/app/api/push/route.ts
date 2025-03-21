import { NextRequest, NextResponse } from "next/server";

export async function POST(req: NextRequest) {
  console.log("0------------");
  const subscription = await req.json();

  console.log("1------------", subscription);

  const res = await fetch(
    `${process.env.NEXT_PUBLIC_API_BASE_URL}/push/subscribe`,
    {
      method: "POST",
      body: JSON.stringify(subscription),
      headers: { "Content-Type": "application/json" },
    }
  );
  console.log("2----------", res);
  if (!res.ok) {
    return NextResponse.json({ error: "Subscription failed" }, { status: 500 });
  }

  return NextResponse.json({ message: "Subscribed successfully!" });
}
