import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 50,
  duration: '30s',
};

export default function () {
  const timestamp = Math.floor(Date.now() / 1000);

  const body = JSON.stringify({
    nsu: "123456",
    amount: 199.90,
    terminalId: "T-1000"
  });

  const payload = timestamp + body;

  const signature = sha256(payload, "super-secret");

  const res = http.post('http://localhost:5000/v1/pos/transactions/authorize',
    body,
    {
      headers: {
        'Content-Type': 'application/json',
        'X-Timestamp': timestamp,
        'X-Signature': signature
      },
    }
  );

  check(res, {
    'status is 200': (r) => r.status === 200,
  });
}

function sha256(payload, key) {
  return payload;
}