import { html as c, property as f, state as N, customElement as y } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as R } from "@umbraco-cms/backoffice/lit-element";
import { c as g } from "./client.gen-Crtm1jAh.js";
var a = /* @__PURE__ */ ((t) => (t.NOT_FOUND = "NotFound", t.UNKNOWN = "Unknown", t.PREPARING = "Preparing", t.ERRORED = "Errored", t.READY = "Ready", t))(a || {});
class m {
  static getStatus(e) {
    return ((e == null ? void 0 : e.client) ?? g).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umux/api/v1/status",
      ...e
    });
  }
}
var O = Object.defineProperty, P = Object.getOwnPropertyDescriptor, E = (t) => {
  throw TypeError(t);
}, l = (t, e, s, o) => {
  for (var r = o > 1 ? void 0 : o ? P(e, s) : e, d = t.length - 1, h; d >= 0; d--)
    (h = t[d]) && (r = (o ? h(e, s, r) : h(r)) || r);
  return o && r && O(e, s, r), r;
}, U = (t, e, s) => e.has(t) || E("Cannot " + s), i = (t, e, s) => (U(t, e, "read from private field"), s ? s.call(t) : e.get(t)), p = (t, e, s) => e.has(t) ? E("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, s), n, _, v;
let u = class extends R {
  constructor() {
    super(...arguments), this._value = null, this._status = null, this._statusLoading = !1, p(this, n, async () => {
      var e;
      if (!((e = this.value) != null && e.MuxAssetId)) {
        this._status = null;
        return;
      }
      this._statusLoading = !0;
      const { data: t } = await m.getStatus({ query: { assetId: this.value.MuxAssetId } });
      this._statusLoading = !1, this._status = t, (this._status === a.UNKNOWN || this._status === a.PREPARING) && setTimeout(() => i(this, n).call(this), 5e3);
    }), p(this, _, (t) => {
      switch (t) {
        case a.PREPARING:
          return "warning";
        case a.ERRORED:
          return "danger";
        case a.READY:
          return "positive";
        case a.NOT_FOUND:
        case a.UNKNOWN:
        default:
          return "default";
      }
    }), p(this, v, (t) => {
      switch (t) {
        case a.PREPARING:
          return "Preparing";
        case a.ERRORED:
          return "Errored";
        case a.READY:
          return "Ready";
        case a.NOT_FOUND:
          return "Not Found";
        case a.UNKNOWN:
        default:
          return "Unknown";
      }
    });
  }
  set value(t) {
    this._value = t, i(this, n).call(this);
  }
  get value() {
    return this._value;
  }
  render() {
    var t, e;
    return this.value ? c`
          <uui-ref-node name=${((t = this.value) == null ? void 0 : t.MuxAssetId) ?? "No asset id"} detail=${((e = this.value) == null ? void 0 : e.PlaybackId) ?? "No playback id"} readonly>
            <uui-icon slot="icon" name="icon-video"></uui-icon>
            ${this._statusLoading ? c`<uui-loader size="s" slot="tag"></uui-loader>` : c`<uui-tag size="s" slot="tag" color=${i(this, _).call(this, this._status)} @click=${i(this, n)}>${i(this, v).call(this, this._status)}</uui-tag>`}

          </uui-ref-node>
        ` : c`<uui-tag look="placeholder">No video uploaded to MUX</uui-tag>`;
  }
};
n = /* @__PURE__ */ new WeakMap();
_ = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
l([
  f()
], u.prototype, "value", 1);
l([
  N()
], u.prototype, "_value", 2);
l([
  N()
], u.prototype, "_status", 2);
l([
  N()
], u.prototype, "_statusLoading", 2);
u = l([
  y("umux-property-editor-ui-mux-sync")
], u);
const I = u;
export {
  I as default,
  u as uMuxPropertyEditorUIMuxSyncElement
};
//# sourceMappingURL=property-editor-ui-mux-sync.element-CxO4A1Bf.js.map
