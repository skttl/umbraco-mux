const I = {
  bodySerializer: (n) => JSON.stringify(
    n,
    (r, e) => typeof e == "bigint" ? e.toString() : e
  )
}, E = async (n, r) => {
  const e = typeof r == "function" ? await r(n) : r;
  if (e)
    return n.scheme === "bearer" ? `Bearer ${e}` : n.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, _ = (n) => {
  switch (n) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, U = (n) => {
  switch (n) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, T = (n) => {
  switch (n) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, $ = ({
  allowReserved: n,
  explode: r,
  name: e,
  style: a,
  value: o
}) => {
  if (!r) {
    const t = (n ? o : o.map((c) => encodeURIComponent(c))).join(U(a));
    switch (a) {
      case "label":
        return `.${t}`;
      case "matrix":
        return `;${e}=${t}`;
      case "simple":
        return t;
      default:
        return `${e}=${t}`;
    }
  }
  const i = _(a), s = o.map((t) => a === "label" || a === "simple" ? n ? t : encodeURIComponent(t) : y({
    allowReserved: n,
    name: e,
    value: t
  })).join(i);
  return a === "label" || a === "matrix" ? i + s : s;
}, y = ({
  allowReserved: n,
  name: r,
  value: e
}) => {
  if (e == null)
    return "";
  if (typeof e == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${r}=${n ? e : encodeURIComponent(e)}`;
}, z = ({
  allowReserved: n,
  explode: r,
  name: e,
  style: a,
  value: o,
  valueOnly: i
}) => {
  if (o instanceof Date)
    return i ? o.toISOString() : `${e}=${o.toISOString()}`;
  if (a !== "deepObject" && !r) {
    let c = [];
    Object.entries(o).forEach(([d, b]) => {
      c = [
        ...c,
        d,
        n ? b : encodeURIComponent(b)
      ];
    });
    const l = c.join(",");
    switch (a) {
      case "form":
        return `${e}=${l}`;
      case "label":
        return `.${l}`;
      case "matrix":
        return `;${e}=${l}`;
      default:
        return l;
    }
  }
  const s = T(a), t = Object.entries(o).map(
    ([c, l]) => y({
      allowReserved: n,
      name: a === "deepObject" ? `${e}[${c}]` : c,
      value: l
    })
  ).join(s);
  return a === "label" || a === "matrix" ? s + t : t;
}, P = /\{[^{}]+\}/g, W = ({ path: n, url: r }) => {
  let e = r;
  const a = r.match(P);
  if (a)
    for (const o of a) {
      let i = !1, s = o.substring(1, o.length - 1), t = "simple";
      s.endsWith("*") && (i = !0, s = s.substring(0, s.length - 1)), s.startsWith(".") ? (s = s.substring(1), t = "label") : s.startsWith(";") && (s = s.substring(1), t = "matrix");
      const c = n[s];
      if (c == null)
        continue;
      if (Array.isArray(c)) {
        e = e.replace(
          o,
          $({ explode: i, name: s, style: t, value: c })
        );
        continue;
      }
      if (typeof c == "object") {
        e = e.replace(
          o,
          z({
            explode: i,
            name: s,
            style: t,
            value: c,
            valueOnly: !0
          })
        );
        continue;
      }
      if (t === "matrix") {
        e = e.replace(
          o,
          `;${y({
            name: s,
            value: c
          })}`
        );
        continue;
      }
      const l = encodeURIComponent(
        t === "label" ? `.${c}` : c
      );
      e = e.replace(o, l);
    }
  return e;
}, O = ({
  allowReserved: n,
  array: r,
  object: e
} = {}) => (o) => {
  const i = [];
  if (o && typeof o == "object")
    for (const s in o) {
      const t = o[s];
      if (t != null)
        if (Array.isArray(t)) {
          const c = $({
            allowReserved: n,
            explode: !0,
            name: s,
            style: "form",
            value: t,
            ...r
          });
          c && i.push(c);
        } else if (typeof t == "object") {
          const c = z({
            allowReserved: n,
            explode: !0,
            name: s,
            style: "deepObject",
            value: t,
            ...e
          });
          c && i.push(c);
        } else {
          const c = y({
            allowReserved: n,
            name: s,
            value: t
          });
          c && i.push(c);
        }
    }
  return i.join("&");
}, k = (n) => {
  var e;
  if (!n)
    return "stream";
  const r = (e = n.split(";")[0]) == null ? void 0 : e.trim();
  if (r) {
    if (r.startsWith("application/json") || r.endsWith("+json"))
      return "json";
    if (r === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (a) => r.startsWith(a)
    ))
      return "blob";
    if (r.startsWith("text/"))
      return "text";
  }
}, V = async ({
  security: n,
  ...r
}) => {
  for (const e of n) {
    const a = await E(e, r.auth);
    if (!a)
      continue;
    const o = e.name ?? "Authorization";
    switch (e.in) {
      case "query":
        r.query || (r.query = {}), r.query[o] = a;
        break;
      case "cookie":
        r.headers.append("Cookie", `${o}=${a}`);
        break;
      case "header":
      default:
        r.headers.set(o, a);
        break;
    }
    return;
  }
}, w = (n) => D({
  baseUrl: n.baseUrl,
  path: n.path,
  query: n.query,
  querySerializer: typeof n.querySerializer == "function" ? n.querySerializer : O(n.querySerializer),
  url: n.url
}), D = ({
  baseUrl: n,
  path: r,
  query: e,
  querySerializer: a,
  url: o
}) => {
  const i = o.startsWith("/") ? o : `/${o}`;
  let s = (n ?? "") + i;
  r && (s = W({ path: r, url: s }));
  let t = e ? a(e) : "";
  return t.startsWith("?") && (t = t.substring(1)), t && (s += `?${t}`), s;
}, A = (n, r) => {
  var a;
  const e = { ...n, ...r };
  return (a = e.baseUrl) != null && a.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = C(n.headers, r.headers), e;
}, C = (...n) => {
  const r = new Headers();
  for (const e of n) {
    if (!e || typeof e != "object")
      continue;
    const a = e instanceof Headers ? e.entries() : Object.entries(e);
    for (const [o, i] of a)
      if (i === null)
        r.delete(o);
      else if (Array.isArray(i))
        for (const s of i)
          r.append(o, s);
      else i !== void 0 && r.set(
        o,
        typeof i == "object" ? JSON.stringify(i) : i
      );
  }
  return r;
};
class g {
  constructor() {
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(r) {
    return typeof r == "number" ? this._fns[r] ? r : -1 : this._fns.indexOf(r);
  }
  exists(r) {
    const e = this.getInterceptorIndex(r);
    return !!this._fns[e];
  }
  eject(r) {
    const e = this.getInterceptorIndex(r);
    this._fns[e] && (this._fns[e] = null);
  }
  update(r, e) {
    const a = this.getInterceptorIndex(r);
    return this._fns[a] ? (this._fns[a] = e, r) : !1;
  }
  use(r) {
    return this._fns = [...this._fns, r], this._fns.length - 1;
  }
}
const H = () => ({
  error: new g(),
  request: new g(),
  response: new g()
}), N = O({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), R = {
  "Content-Type": "application/json"
}, q = (n = {}) => ({
  ...I,
  headers: R,
  parseAs: "auto",
  querySerializer: N,
  ...n
}), B = (n = {}) => {
  let r = A(q(), n);
  const e = () => ({ ...r }), a = (s) => (r = A(r, s), e()), o = H(), i = async (s) => {
    const t = {
      ...r,
      ...s,
      fetch: s.fetch ?? r.fetch ?? globalThis.fetch,
      headers: C(r.headers, s.headers)
    };
    t.security && await V({
      ...t,
      security: t.security
    }), t.requestValidator && await t.requestValidator(t), t.body && t.bodySerializer && (t.body = t.bodySerializer(t.body)), (t.body === void 0 || t.body === "") && t.headers.delete("Content-Type");
    const c = w(t), l = {
      redirect: "follow",
      ...t
    };
    let d = new Request(c, l);
    for (const u of o.request._fns)
      u && (d = await u(d, t));
    const b = t.fetch;
    let f = await b(d);
    for (const u of o.response._fns)
      u && (f = await u(f, d, t));
    const m = {
      request: d,
      response: f
    };
    if (f.ok) {
      if (f.status === 204 || f.headers.get("Content-Length") === "0")
        return t.responseStyle === "data" ? {} : {
          data: {},
          ...m
        };
      const u = (t.parseAs === "auto" ? k(f.headers.get("Content-Type")) : t.parseAs) ?? "json";
      let h;
      switch (u) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          h = await f[u]();
          break;
        case "stream":
          return t.responseStyle === "data" ? f.body : {
            data: f.body,
            ...m
          };
      }
      return u === "json" && (t.responseValidator && await t.responseValidator(h), t.responseTransformer && (h = await t.responseTransformer(h))), t.responseStyle === "data" ? h : {
        data: h,
        ...m
      };
    }
    const j = await f.text();
    let x;
    try {
      x = JSON.parse(j);
    } catch {
    }
    const S = x ?? j;
    let p = S;
    for (const u of o.error._fns)
      u && (p = await u(S, f, d, t));
    if (p = p || {}, t.throwOnError)
      throw p;
    return t.responseStyle === "data" ? void 0 : {
      error: p,
      ...m
    };
  };
  return {
    buildUrl: w,
    connect: (s) => i({ ...s, method: "CONNECT" }),
    delete: (s) => i({ ...s, method: "DELETE" }),
    get: (s) => i({ ...s, method: "GET" }),
    getConfig: e,
    head: (s) => i({ ...s, method: "HEAD" }),
    interceptors: o,
    options: (s) => i({ ...s, method: "OPTIONS" }),
    patch: (s) => i({ ...s, method: "PATCH" }),
    post: (s) => i({ ...s, method: "POST" }),
    put: (s) => i({ ...s, method: "PUT" }),
    request: i,
    setConfig: a,
    trace: (s) => i({ ...s, method: "TRACE" })
  };
}, J = B(q({
  baseUrl: "http://localhost:59927"
}));
export {
  J as c
};
//# sourceMappingURL=client.gen-Crtm1jAh.js.map
