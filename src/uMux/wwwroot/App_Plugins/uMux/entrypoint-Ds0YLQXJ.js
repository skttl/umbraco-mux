import { UMB_AUTH_CONTEXT as i } from "@umbraco-cms/backoffice/auth";
import { c as n } from "./client.gen-Crtm1jAh.js";
const a = (r, o) => {
  r.consumeContext(i, async (s) => {
    const e = s == null ? void 0 : s.getOpenApiConfiguration();
    n.setConfig({
      auth: (e == null ? void 0 : e.token) ?? void 0,
      baseUrl: (e == null ? void 0 : e.base) ?? "",
      credentials: (e == null ? void 0 : e.credentials) ?? "same-origin"
    });
  });
};
export {
  a as onInit
};
//# sourceMappingURL=entrypoint-Ds0YLQXJ.js.map
