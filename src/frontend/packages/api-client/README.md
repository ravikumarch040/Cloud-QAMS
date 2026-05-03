# QAMS API Client

Generated or hand-authored API clients for BFF and service contracts will live here.

Rules:

- Always send `X-Tenant-Id`.
- Always send `X-Correlation-Id`.
- Send `Idempotency-Key` for mutating requests.
- Preserve API response envelopes.
- Surface validation and compliance errors directly to workflow UI.
