const Buffer=function(){const t=(()=>{for(var t=[],r=[],e="undefined"!=typeof Uint8Array?Uint8Array:Array,n="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/",o=0,i=n.length;o<i;++o)t[o]=n[o],r[n.charCodeAt(o)]=o;function f(t){var r=t.length;if(r%4>0)throw new Error("Invalid string. Length must be a multiple of 4");var e=t.indexOf("=");return-1===e&&(e=r),[e,e===r?0:4-e%4]}function s(r,e,n){for(var o,i,f=[],s=e;s<n;s+=3)o=(r[s]<<16&16711680)+(r[s+1]<<8&65280)+(255&r[s+2]),f.push(t[(i=o)>>18&63]+t[i>>12&63]+t[i>>6&63]+t[63&i]);return f.join("")}return r["-".charCodeAt(0)]=62,r["_".charCodeAt(0)]=63,{byteLength:function(t){var r=f(t),e=r[0],n=r[1];return 3*(e+n)/4-n},toByteArray:function(t){var n,o,i=f(t),s=i[0],u=i[1],h=new e(function(t,r,e){return 3*(r+e)/4-e}(0,s,u)),a=0,c=u>0?s-4:s;for(o=0;o<c;o+=4)n=r[t.charCodeAt(o)]<<18|r[t.charCodeAt(o+1)]<<12|r[t.charCodeAt(o+2)]<<6|r[t.charCodeAt(o+3)],h[a++]=n>>16&255,h[a++]=n>>8&255,h[a++]=255&n;return 2===u&&(n=r[t.charCodeAt(o)]<<2|r[t.charCodeAt(o+1)]>>4,h[a++]=255&n),1===u&&(n=r[t.charCodeAt(o)]<<10|r[t.charCodeAt(o+1)]<<4|r[t.charCodeAt(o+2)]>>2,h[a++]=n>>8&255,h[a++]=255&n),h},fromByteArray:function(r){for(var e,n=r.length,o=n%3,i=[],f=0,u=n-o;f<u;f+=16383)i.push(s(r,f,f+16383>u?u:f+16383));return 1===o?(e=r[n-1],i.push(t[e>>2]+t[e<<4&63]+"==")):2===o&&(e=(r[n-2]<<8)+r[n-1],i.push(t[e>>10]+t[e>>4&63]+t[e<<2&63]+"=")),i.join("")}}})(),r={read:function(t,r,e,n,o){let i,f;const s=8*o-n-1,u=(1<<s)-1,h=u>>1;let a=-7,c=e?o-1:0;const l=e?-1:1;let p=t[r+c];for(c+=l,i=p&(1<<-a)-1,p>>=-a,a+=s;a>0;)i=256*i+t[r+c],c+=l,a-=8;for(f=i&(1<<-a)-1,i>>=-a,a+=n;a>0;)f=256*f+t[r+c],c+=l,a-=8;if(0===i)i=1-h;else{if(i===u)return f?NaN:1/0*(p?-1:1);f+=Math.pow(2,n),i-=h}return(p?-1:1)*f*Math.pow(2,i-n)},write:function(t,r,e,n,o,i){let f,s,u,h=8*i-o-1;const a=(1<<h)-1,c=a>>1,l=23===o?Math.pow(2,-24)-Math.pow(2,-77):0;let p=n?0:i-1;const g=n?1:-1,y=r<0||0===r&&1/r<0?1:0;for(r=Math.abs(r),isNaN(r)||r===1/0?(s=isNaN(r)?1:0,f=a):(f=Math.floor(Math.log(r)/Math.LN2),r*(u=Math.pow(2,-f))<1&&(f--,u*=2),(r+=f+c>=1?l/u:l*Math.pow(2,1-c))*u>=2&&(f++,u/=2),f+c>=a?(s=0,f=a):f+c>=1?(s=(r*u-1)*Math.pow(2,o),f+=c):(s=r*Math.pow(2,c-1)*Math.pow(2,o),f=0));o>=8;)t[e+p]=255&s,p+=g,s/=256,o-=8;for(f=f<<o|s,h+=o;h>0;)t[e+p]=255&f,p+=g,f/=256,h-=8;t[e+p-g]|=128*y}},e="function"==typeof Symbol&&"function"==typeof Symbol.for?Symbol.for("nodejs.util.inspect.custom"):null,n=2147483647;function o(t){if(t>n)throw new RangeError('The value "'+t+'" is invalid for option "size"');const r=new Uint8Array(t);return Object.setPrototypeOf(r,i.prototype),r}function i(t,r,e){if("number"==typeof t){if("string"==typeof r)throw new TypeError('The "string" argument must be of type string. Received type number');return u(t)}return f(t,r,e)}function f(t,r,e){if("string"==typeof t)return function(t,r){"string"==typeof r&&""!==r||(r="utf8");if(!i.isEncoding(r))throw new TypeError("Unknown encoding: "+r);const e=0|l(t,r);let n=o(e);const f=n.write(t,r);f!==e&&(n=n.slice(0,f));return n}(t,r);if(ArrayBuffer.isView(t))return function(t){if(q(t,Uint8Array)){const r=new Uint8Array(t);return a(r.buffer,r.byteOffset,r.byteLength)}return h(t)}(t);if(null==t)throw new TypeError("The first argument must be one of type string, Buffer, ArrayBuffer, Array, or Array-like Object. Received type "+typeof t);if(q(t,ArrayBuffer)||t&&q(t.buffer,ArrayBuffer))return a(t,r,e);if("undefined"!=typeof SharedArrayBuffer&&(q(t,SharedArrayBuffer)||t&&q(t.buffer,SharedArrayBuffer)))return a(t,r,e);if("number"==typeof t)throw new TypeError('The "value" argument must not be of type number. Received type number');const n=t.valueOf&&t.valueOf();if(null!=n&&n!==t)return i.from(n,r,e);const f=function(t){if(i.isBuffer(t)){const r=0|c(t.length),e=o(r);return 0===e.length?e:(t.copy(e,0,0,r),e)}if(void 0!==t.length)return"number"!=typeof t.length||K(t.length)?o(0):h(t);if("Buffer"===t.type&&Array.isArray(t.data))return h(t.data)}(t);if(f)return f;if("undefined"!=typeof Symbol&&null!=Symbol.toPrimitive&&"function"==typeof t[Symbol.toPrimitive])return i.from(t[Symbol.toPrimitive]("string"),r,e);throw new TypeError("The first argument must be one of type string, Buffer, ArrayBuffer, Array, or Array-like Object. Received type "+typeof t)}function s(t){if("number"!=typeof t)throw new TypeError('"size" argument must be of type number');if(t<0)throw new RangeError('The value "'+t+'" is invalid for option "size"')}function u(t){return s(t),o(t<0?0:0|c(t))}function h(t){const r=t.length<0?0:0|c(t.length),e=o(r);for(let n=0;n<r;n+=1)e[n]=255&t[n];return e}function a(t,r,e){if(r<0||t.byteLength<r)throw new RangeError('"offset" is outside of buffer bounds');if(t.byteLength<r+(e||0))throw new RangeError('"length" is outside of buffer bounds');let n;return n=void 0===r&&void 0===e?new Uint8Array(t):void 0===e?new Uint8Array(t,r):new Uint8Array(t,r,e),Object.setPrototypeOf(n,i.prototype),n}function c(t){if(t>=n)throw new RangeError("Attempt to allocate Buffer larger than maximum size: 0x"+n.toString(16)+" bytes");return 0|t}function l(t,r){if(i.isBuffer(t))return t.length;if(ArrayBuffer.isView(t)||q(t,ArrayBuffer))return t.byteLength;if("string"!=typeof t)throw new TypeError('The "string" argument must be one of type string, Buffer, or ArrayBuffer. Received type '+typeof t);const e=t.length,n=arguments.length>2&&!0===arguments[2];if(!n&&0===e)return 0;let o=!1;for(;;)switch(r){case"ascii":case"latin1":case"binary":return e;case"utf8":case"utf-8":return z(t).length;case"ucs2":case"ucs-2":case"utf16le":case"utf-16le":return 2*e;case"hex":return e>>>1;case"base64":return Y(t).length;default:if(o)return n?-1:z(t).length;r=(""+r).toLowerCase(),o=!0}}function p(t,r,e){const n=t[r];t[r]=t[e],t[e]=n}function g(t,r,e,n,o){if(0===t.length)return-1;if("string"==typeof e?(n=e,e=0):e>2147483647?e=2147483647:e<-2147483648&&(e=-2147483648),K(e=+e)&&(e=o?0:t.length-1),e<0&&(e=t.length+e),e>=t.length){if(o)return-1;e=t.length-1}else if(e<0){if(!o)return-1;e=0}if("string"==typeof r&&(r=i.from(r,n)),i.isBuffer(r))return 0===r.length?-1:y(t,r,e,n,o);if("number"==typeof r)return r&=255,"function"==typeof Uint8Array.prototype.indexOf?o?Uint8Array.prototype.indexOf.call(t,r,e):Uint8Array.prototype.lastIndexOf.call(t,r,e):y(t,[r],e,n,o);throw new TypeError("val must be string, number or Buffer")}function y(t,r,e,n,o){let i,f=1,s=t.length,u=r.length;if(void 0!==n&&("ucs2"===(n=String(n).toLowerCase())||"ucs-2"===n||"utf16le"===n||"utf-16le"===n)){if(t.length<2||r.length<2)return-1;f=2,s/=2,u/=2,e/=2}function h(t,r){return 1===f?t[r]:t.readUInt16BE(r*f)}if(o){let n=-1;for(i=e;i<s;i++)if(h(t,i)===h(r,-1===n?0:i-n)){if(-1===n&&(n=i),i-n+1===u)return n*f}else-1!==n&&(i-=i-n),n=-1}else for(e+u>s&&(e=s-u),i=e;i>=0;i--){let e=!0;for(let n=0;n<u;n++)if(h(t,i+n)!==h(r,n)){e=!1;break}if(e)return i}return-1}function d(t,r,e,n){e=Number(e)||0;const o=t.length-e;n?(n=Number(n))>o&&(n=o):n=o;const i=r.length;let f;for(n>i/2&&(n=i/2),f=0;f<n;++f){const n=parseInt(r.substr(2*f,2),16);if(K(n))return f;t[e+f]=n}return f}function w(t,r,e,n){return G(z(r,t.length-e),t,e,n)}function b(t,r,e,n){return G(function(t){const r=[];for(let e=0;e<t.length;++e)r.push(255&t.charCodeAt(e));return r}(r),t,e,n)}function E(t,r,e,n){return G(Y(r),t,e,n)}function m(t,r,e,n){return G(function(t,r){let e,n,o;const i=[];for(let f=0;f<t.length&&!((r-=2)<0);++f)e=t.charCodeAt(f),n=e>>8,o=e%256,i.push(o),i.push(n);return i}(r,t.length-e),t,e,n)}function B(r,e,n){return 0===e&&n===r.length?t.fromByteArray(r):t.fromByteArray(r.slice(e,n))}function A(t,r,e){e=Math.min(t.length,e);const n=[];let o=r;for(;o<e;){const r=t[o];let i=null,f=r>239?4:r>223?3:r>191?2:1;if(o+f<=e){let e,n,s,u;switch(f){case 1:r<128&&(i=r);break;case 2:128==(192&(e=t[o+1]))&&(u=(31&r)<<6|63&e)>127&&(i=u);break;case 3:e=t[o+1],n=t[o+2],128==(192&e)&&128==(192&n)&&(u=(15&r)<<12|(63&e)<<6|63&n)>2047&&(u<55296||u>57343)&&(i=u);break;case 4:e=t[o+1],n=t[o+2],s=t[o+3],128==(192&e)&&128==(192&n)&&128==(192&s)&&(u=(15&r)<<18|(63&e)<<12|(63&n)<<6|63&s)>65535&&u<1114112&&(i=u)}}null===i?(i=65533,f=1):i>65535&&(i-=65536,n.push(i>>>10&1023|55296),i=56320|1023&i),n.push(i),o+=f}return function(t){const r=t.length;if(r<=I)return String.fromCharCode.apply(String,t);let e="",n=0;for(;n<r;)e+=String.fromCharCode.apply(String,t.slice(n,n+=I));return e}(n)}i.TYPED_ARRAY_SUPPORT=function(){try{const t=new Uint8Array(1),r={foo:function(){return 42}};return Object.setPrototypeOf(r,Uint8Array.prototype),Object.setPrototypeOf(t,r),42===t.foo()}catch(t){return!1}}(),i.TYPED_ARRAY_SUPPORT||"undefined"==typeof console||"function"!=typeof console.error||console.error("This browser lacks typed array (Uint8Array) support which is required by `buffer` v5.x. Use `buffer` v4.x if you require old browser support."),Object.defineProperty(i.prototype,"parent",{enumerable:!0,get:function(){if(i.isBuffer(this))return this.buffer}}),Object.defineProperty(i.prototype,"offset",{enumerable:!0,get:function(){if(i.isBuffer(this))return this.byteOffset}}),i.poolSize=8192,i.from=function(t,r,e){return f(t,r,e)},Object.setPrototypeOf(i.prototype,Uint8Array.prototype),Object.setPrototypeOf(i,Uint8Array),i.alloc=function(t,r,e){return function(t,r,e){return s(t),t<=0?o(t):void 0!==r?"string"==typeof e?o(t).fill(r,e):o(t).fill(r):o(t)}(t,r,e)},i.allocUnsafe=function(t){return u(t)},i.allocUnsafeSlow=function(t){return u(t)},i.isBuffer=function(t){return null!=t&&!0===t._isBuffer&&t!==i.prototype},i.compare=function(t,r){if(q(t,Uint8Array)&&(t=i.from(t,t.offset,t.byteLength)),q(r,Uint8Array)&&(r=i.from(r,r.offset,r.byteLength)),!i.isBuffer(t)||!i.isBuffer(r))throw new TypeError('The "buf1", "buf2" arguments must be one of type Buffer or Uint8Array');if(t===r)return 0;let e=t.length,n=r.length;for(let o=0,i=Math.min(e,n);o<i;++o)if(t[o]!==r[o]){e=t[o],n=r[o];break}return e<n?-1:n<e?1:0},i.isEncoding=function(t){switch(String(t).toLowerCase()){case"hex":case"utf8":case"utf-8":case"ascii":case"latin1":case"binary":case"base64":case"ucs2":case"ucs-2":case"utf16le":case"utf-16le":return!0;default:return!1}},i.concat=function(t,r){if(!Array.isArray(t))throw new TypeError('"list" argument must be an Array of Buffers');if(0===t.length)return i.alloc(0);let e;if(void 0===r)for(r=0,e=0;e<t.length;++e)r+=t[e].length;const n=i.allocUnsafe(r);let o=0;for(e=0;e<t.length;++e){let r=t[e];if(q(r,Uint8Array))o+r.length>n.length?(i.isBuffer(r)||(r=i.from(r.buffer,r.byteOffset,r.byteLength)),r.copy(n,o)):Uint8Array.prototype.set.call(n,r,o);else{if(!i.isBuffer(r))throw new TypeError('"list" argument must be an Array of Buffers');r.copy(n,o)}o+=r.length}return n},i.byteLength=l,i.prototype._isBuffer=!0,i.prototype.swap16=function(){const t=this.length;if(t%2!=0)throw new RangeError("Buffer size must be a multiple of 16-bits");for(let r=0;r<t;r+=2)p(this,r,r+1);return this},i.prototype.swap32=function(){const t=this.length;if(t%4!=0)throw new RangeError("Buffer size must be a multiple of 32-bits");for(let r=0;r<t;r+=4)p(this,r,r+3),p(this,r+1,r+2);return this},i.prototype.swap64=function(){const t=this.length;if(t%8!=0)throw new RangeError("Buffer size must be a multiple of 64-bits");for(let r=0;r<t;r+=8)p(this,r,r+7),p(this,r+1,r+6),p(this,r+2,r+5),p(this,r+3,r+4);return this},i.prototype.toString=function(){const t=this.length;return 0===t?"":0===arguments.length?A(this,0,t):function(t,r,e){let n=!1;if((void 0===r||r<0)&&(r=0),r>this.length)return"";if((void 0===e||e>this.length)&&(e=this.length),e<=0)return"";if((e>>>=0)<=(r>>>=0))return"";for(t||(t="utf8");;)switch(t){case"hex":return v(this,r,e);case"utf8":case"utf-8":return A(this,r,e);case"ascii":return L(this,r,e);case"latin1":case"binary":return U(this,r,e);case"base64":return B(this,r,e);case"ucs2":case"ucs-2":case"utf16le":case"utf-16le":return R(this,r,e);default:if(n)throw new TypeError("Unknown encoding: "+t);t=(t+"").toLowerCase(),n=!0}}.apply(this,arguments)},i.prototype.toLocaleString=i.prototype.toString,i.prototype.equals=function(t){if(!i.isBuffer(t))throw new TypeError("Argument must be a Buffer");return this===t||0===i.compare(this,t)},i.prototype.inspect=function(){let t="";const r=exports.INSPECT_MAX_BYTES;return t=this.toString("hex",0,r).replace(/(.{2})/g,"$1 ").trim(),this.length>r&&(t+=" ... "),"<Buffer "+t+">"},e&&(i.prototype[e]=i.prototype.inspect),i.prototype.compare=function(t,r,e,n,o){if(q(t,Uint8Array)&&(t=i.from(t,t.offset,t.byteLength)),!i.isBuffer(t))throw new TypeError('The "target" argument must be one of type Buffer or Uint8Array. Received type '+typeof t);if(void 0===r&&(r=0),void 0===e&&(e=t?t.length:0),void 0===n&&(n=0),void 0===o&&(o=this.length),r<0||e>t.length||n<0||o>this.length)throw new RangeError("out of range index");if(n>=o&&r>=e)return 0;if(n>=o)return-1;if(r>=e)return 1;if(this===t)return 0;let f=(o>>>=0)-(n>>>=0),s=(e>>>=0)-(r>>>=0);const u=Math.min(f,s),h=this.slice(n,o),a=t.slice(r,e);for(let t=0;t<u;++t)if(h[t]!==a[t]){f=h[t],s=a[t];break}return f<s?-1:s<f?1:0},i.prototype.includes=function(t,r,e){return-1!==this.indexOf(t,r,e)},i.prototype.indexOf=function(t,r,e){return g(this,t,r,e,!0)},i.prototype.lastIndexOf=function(t,r,e){return g(this,t,r,e,!1)},i.prototype.write=function(t,r,e,n){if(void 0===r)n="utf8",e=this.length,r=0;else if(void 0===e&&"string"==typeof r)n=r,e=this.length,r=0;else{if(!isFinite(r))throw new Error("Buffer.write(string, encoding, offset[, length]) is no longer supported");r>>>=0,isFinite(e)?(e>>>=0,void 0===n&&(n="utf8")):(n=e,e=void 0)}const o=this.length-r;if((void 0===e||e>o)&&(e=o),t.length>0&&(e<0||r<0)||r>this.length)throw new RangeError("Attempt to write outside buffer bounds");n||(n="utf8");let i=!1;for(;;)switch(n){case"hex":return d(this,t,r,e);case"utf8":case"utf-8":return w(this,t,r,e);case"ascii":case"latin1":case"binary":return b(this,t,r,e);case"base64":return E(this,t,r,e);case"ucs2":case"ucs-2":case"utf16le":case"utf-16le":return m(this,t,r,e);default:if(i)throw new TypeError("Unknown encoding: "+n);n=(""+n).toLowerCase(),i=!0}},i.prototype.toJSON=function(){return{type:"Buffer",data:Array.prototype.slice.call(this._arr||this,0)}};const I=4096;function L(t,r,e){let n="";e=Math.min(t.length,e);for(let o=r;o<e;++o)n+=String.fromCharCode(127&t[o]);return n}function U(t,r,e){let n="";e=Math.min(t.length,e);for(let o=r;o<e;++o)n+=String.fromCharCode(t[o]);return n}function v(t,r,e){const n=t.length;(!r||r<0)&&(r=0),(!e||e<0||e>n)&&(e=n);let o="";for(let n=r;n<e;++n)o+=V[t[n]];return o}function R(t,r,e){const n=t.slice(r,e);let o="";for(let t=0;t<n.length-1;t+=2)o+=String.fromCharCode(n[t]+256*n[t+1]);return o}function T(t,r,e){if(t%1!=0||t<0)throw new RangeError("offset is not uint");if(t+r>e)throw new RangeError("Trying to access beyond buffer length")}function O(t,r,e,n,o,f){if(!i.isBuffer(t))throw new TypeError('"buffer" argument must be a Buffer instance');if(r>o||r<f)throw new RangeError('"value" argument is out of bounds');if(e+n>t.length)throw new RangeError("Index out of range")}function S(t,r,e,n,o){$(r,n,o,t,e,7);let i=Number(r&BigInt(4294967295));t[e++]=i,i>>=8,t[e++]=i,i>>=8,t[e++]=i,i>>=8,t[e++]=i;let f=Number(r>>BigInt(32)&BigInt(4294967295));return t[e++]=f,f>>=8,t[e++]=f,f>>=8,t[e++]=f,f>>=8,t[e++]=f,e}function _(t,r,e,n,o){$(r,n,o,t,e,7);let i=Number(r&BigInt(4294967295));t[e+7]=i,i>>=8,t[e+6]=i,i>>=8,t[e+5]=i,i>>=8,t[e+4]=i;let f=Number(r>>BigInt(32)&BigInt(4294967295));return t[e+3]=f,f>>=8,t[e+2]=f,f>>=8,t[e+1]=f,f>>=8,t[e]=f,e+8}function C(t,r,e,n,o,i){if(e+n>t.length)throw new RangeError("Index out of range");if(e<0)throw new RangeError("Index out of range")}function M(t,e,n,o,i){return e=+e,n>>>=0,i||C(t,0,n,4),r.write(t,e,n,o,23,4),n+4}function x(t,e,n,o,i){return e=+e,n>>>=0,i||C(t,0,n,8),r.write(t,e,n,o,52,8),n+8}i.prototype.slice=function(t,r){const e=this.length;(t=~~t)<0?(t+=e)<0&&(t=0):t>e&&(t=e),(r=void 0===r?e:~~r)<0?(r+=e)<0&&(r=0):r>e&&(r=e),r<t&&(r=t);const n=this.subarray(t,r);return Object.setPrototypeOf(n,i.prototype),n},i.prototype.readUintLE=i.prototype.readUIntLE=function(t,r,e){t>>>=0,r>>>=0,e||T(t,r,this.length);let n=this[t],o=1,i=0;for(;++i<r&&(o*=256);)n+=this[t+i]*o;return n},i.prototype.readUintBE=i.prototype.readUIntBE=function(t,r,e){t>>>=0,r>>>=0,e||T(t,r,this.length);let n=this[t+--r],o=1;for(;r>0&&(o*=256);)n+=this[t+--r]*o;return n},i.prototype.readUint8=i.prototype.readUInt8=function(t,r){return t>>>=0,r||T(t,1,this.length),this[t]},i.prototype.readUint16LE=i.prototype.readUInt16LE=function(t,r){return t>>>=0,r||T(t,2,this.length),this[t]|this[t+1]<<8},i.prototype.readUint16BE=i.prototype.readUInt16BE=function(t,r){return t>>>=0,r||T(t,2,this.length),this[t]<<8|this[t+1]},i.prototype.readUint32LE=i.prototype.readUInt32LE=function(t,r){return t>>>=0,r||T(t,4,this.length),(this[t]|this[t+1]<<8|this[t+2]<<16)+16777216*this[t+3]},i.prototype.readUint32BE=i.prototype.readUInt32BE=function(t,r){return t>>>=0,r||T(t,4,this.length),16777216*this[t]+(this[t+1]<<16|this[t+2]<<8|this[t+3])},i.prototype.readBigUInt64LE=W(function(t){j(t>>>=0,"offset");const r=this[t],e=this[t+7];void 0!==r&&void 0!==e||F(t,this.length-8);const n=r+256*this[++t]+65536*this[++t]+this[++t]*2**24,o=this[++t]+256*this[++t]+65536*this[++t]+e*2**24;return BigInt(n)+(BigInt(o)<<BigInt(32))}),i.prototype.readBigUInt64BE=W(function(t){j(t>>>=0,"offset");const r=this[t],e=this[t+7];void 0!==r&&void 0!==e||F(t,this.length-8);const n=r*2**24+65536*this[++t]+256*this[++t]+this[++t],o=this[++t]*2**24+65536*this[++t]+256*this[++t]+e;return(BigInt(n)<<BigInt(32))+BigInt(o)}),i.prototype.readIntLE=function(t,r,e){t>>>=0,r>>>=0,e||T(t,r,this.length);let n=this[t],o=1,i=0;for(;++i<r&&(o*=256);)n+=this[t+i]*o;return n>=(o*=128)&&(n-=Math.pow(2,8*r)),n},i.prototype.readIntBE=function(t,r,e){t>>>=0,r>>>=0,e||T(t,r,this.length);let n=r,o=1,i=this[t+--n];for(;n>0&&(o*=256);)i+=this[t+--n]*o;return i>=(o*=128)&&(i-=Math.pow(2,8*r)),i},i.prototype.readInt8=function(t,r){return t>>>=0,r||T(t,1,this.length),128&this[t]?-1*(255-this[t]+1):this[t]},i.prototype.readInt16LE=function(t,r){t>>>=0,r||T(t,2,this.length);const e=this[t]|this[t+1]<<8;return 32768&e?4294901760|e:e},i.prototype.readInt16BE=function(t,r){t>>>=0,r||T(t,2,this.length);const e=this[t+1]|this[t]<<8;return 32768&e?4294901760|e:e},i.prototype.readInt32LE=function(t,r){return t>>>=0,r||T(t,4,this.length),this[t]|this[t+1]<<8|this[t+2]<<16|this[t+3]<<24},i.prototype.readInt32BE=function(t,r){return t>>>=0,r||T(t,4,this.length),this[t]<<24|this[t+1]<<16|this[t+2]<<8|this[t+3]},i.prototype.readBigInt64LE=W(function(t){j(t>>>=0,"offset");const r=this[t],e=this[t+7];void 0!==r&&void 0!==e||F(t,this.length-8);const n=this[t+4]+256*this[t+5]+65536*this[t+6]+(e<<24);return(BigInt(n)<<BigInt(32))+BigInt(r+256*this[++t]+65536*this[++t]+this[++t]*2**24)}),i.prototype.readBigInt64BE=W(function(t){j(t>>>=0,"offset");const r=this[t],e=this[t+7];void 0!==r&&void 0!==e||F(t,this.length-8);const n=(r<<24)+65536*this[++t]+256*this[++t]+this[++t];return(BigInt(n)<<BigInt(32))+BigInt(this[++t]*2**24+65536*this[++t]+256*this[++t]+e)}),i.prototype.readFloatLE=function(t,e){return t>>>=0,e||T(t,4,this.length),r.read(this,t,!0,23,4)},i.prototype.readFloatBE=function(t,e){return t>>>=0,e||T(t,4,this.length),r.read(this,t,!1,23,4)},i.prototype.readDoubleLE=function(t,e){return t>>>=0,e||T(t,8,this.length),r.read(this,t,!0,52,8)},i.prototype.readDoubleBE=function(t,e){return t>>>=0,e||T(t,8,this.length),r.read(this,t,!1,52,8)},i.prototype.writeUintLE=i.prototype.writeUIntLE=function(t,r,e,n){if(t=+t,r>>>=0,e>>>=0,!n){O(this,t,r,e,Math.pow(2,8*e)-1,0)}let o=1,i=0;for(this[r]=255&t;++i<e&&(o*=256);)this[r+i]=t/o&255;return r+e},i.prototype.writeUintBE=i.prototype.writeUIntBE=function(t,r,e,n){if(t=+t,r>>>=0,e>>>=0,!n){O(this,t,r,e,Math.pow(2,8*e)-1,0)}let o=e-1,i=1;for(this[r+o]=255&t;--o>=0&&(i*=256);)this[r+o]=t/i&255;return r+e},i.prototype.writeUint8=i.prototype.writeUInt8=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,1,255,0),this[r]=255&t,r+1},i.prototype.writeUint16LE=i.prototype.writeUInt16LE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,2,65535,0),this[r]=255&t,this[r+1]=t>>>8,r+2},i.prototype.writeUint16BE=i.prototype.writeUInt16BE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,2,65535,0),this[r]=t>>>8,this[r+1]=255&t,r+2},i.prototype.writeUint32LE=i.prototype.writeUInt32LE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,4,4294967295,0),this[r+3]=t>>>24,this[r+2]=t>>>16,this[r+1]=t>>>8,this[r]=255&t,r+4},i.prototype.writeUint32BE=i.prototype.writeUInt32BE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,4,4294967295,0),this[r]=t>>>24,this[r+1]=t>>>16,this[r+2]=t>>>8,this[r+3]=255&t,r+4},i.prototype.writeBigUInt64LE=W(function(t,r=0){return S(this,t,r,BigInt(0),BigInt("0xffffffffffffffff"))}),i.prototype.writeBigUInt64BE=W(function(t,r=0){return _(this,t,r,BigInt(0),BigInt("0xffffffffffffffff"))}),i.prototype.writeIntLE=function(t,r,e,n){if(t=+t,r>>>=0,!n){const n=Math.pow(2,8*e-1);O(this,t,r,e,n-1,-n)}let o=0,i=1,f=0;for(this[r]=255&t;++o<e&&(i*=256);)t<0&&0===f&&0!==this[r+o-1]&&(f=1),this[r+o]=(t/i>>0)-f&255;return r+e},i.prototype.writeIntBE=function(t,r,e,n){if(t=+t,r>>>=0,!n){const n=Math.pow(2,8*e-1);O(this,t,r,e,n-1,-n)}let o=e-1,i=1,f=0;for(this[r+o]=255&t;--o>=0&&(i*=256);)t<0&&0===f&&0!==this[r+o+1]&&(f=1),this[r+o]=(t/i>>0)-f&255;return r+e},i.prototype.writeInt8=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,1,127,-128),t<0&&(t=255+t+1),this[r]=255&t,r+1},i.prototype.writeInt16LE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,2,32767,-32768),this[r]=255&t,this[r+1]=t>>>8,r+2},i.prototype.writeInt16BE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,2,32767,-32768),this[r]=t>>>8,this[r+1]=255&t,r+2},i.prototype.writeInt32LE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,4,2147483647,-2147483648),this[r]=255&t,this[r+1]=t>>>8,this[r+2]=t>>>16,this[r+3]=t>>>24,r+4},i.prototype.writeInt32BE=function(t,r,e){return t=+t,r>>>=0,e||O(this,t,r,4,2147483647,-2147483648),t<0&&(t=4294967295+t+1),this[r]=t>>>24,this[r+1]=t>>>16,this[r+2]=t>>>8,this[r+3]=255&t,r+4},i.prototype.writeBigInt64LE=W(function(t,r=0){return S(this,t,r,-BigInt("0x8000000000000000"),BigInt("0x7fffffffffffffff"))}),i.prototype.writeBigInt64BE=W(function(t,r=0){return _(this,t,r,-BigInt("0x8000000000000000"),BigInt("0x7fffffffffffffff"))}),i.prototype.writeFloatLE=function(t,r,e){return M(this,t,r,!0,e)},i.prototype.writeFloatBE=function(t,r,e){return M(this,t,r,!1,e)},i.prototype.writeDoubleLE=function(t,r,e){return x(this,t,r,!0,e)},i.prototype.writeDoubleBE=function(t,r,e){return x(this,t,r,!1,e)},i.prototype.copy=function(t,r,e,n){if(!i.isBuffer(t))throw new TypeError("argument should be a Buffer");if(e||(e=0),n||0===n||(n=this.length),r>=t.length&&(r=t.length),r||(r=0),n>0&&n<e&&(n=e),n===e)return 0;if(0===t.length||0===this.length)return 0;if(r<0)throw new RangeError("targetStart out of bounds");if(e<0||e>=this.length)throw new RangeError("Index out of range");if(n<0)throw new RangeError("sourceEnd out of bounds");n>this.length&&(n=this.length),t.length-r<n-e&&(n=t.length-r+e);const o=n-e;return this===t&&"function"==typeof Uint8Array.prototype.copyWithin?this.copyWithin(r,e,n):Uint8Array.prototype.set.call(t,this.subarray(e,n),r),o},i.prototype.fill=function(t,r,e,n){if("string"==typeof t){if("string"==typeof r?(n=r,r=0,e=this.length):"string"==typeof e&&(n=e,e=this.length),void 0!==n&&"string"!=typeof n)throw new TypeError("encoding must be a string");if("string"==typeof n&&!i.isEncoding(n))throw new TypeError("Unknown encoding: "+n);if(1===t.length){const r=t.charCodeAt(0);("utf8"===n&&r<128||"latin1"===n)&&(t=r)}}else"number"==typeof t?t&=255:"boolean"==typeof t&&(t=Number(t));if(r<0||this.length<r||this.length<e)throw new RangeError("Out of range index");if(e<=r)return this;let o;if(r>>>=0,e=void 0===e?this.length:e>>>0,t||(t=0),"number"==typeof t)for(o=r;o<e;++o)this[o]=t;else{const f=i.isBuffer(t)?t:i.from(t,n),s=f.length;if(0===s)throw new TypeError('The value "'+t+'" is invalid for argument "value"');for(o=0;o<e-r;++o)this[o+r]=f[o%s]}return this};const P={};function k(t,r,e){P[t]=class extends e{constructor(){super(),Object.defineProperty(this,"message",{value:r.apply(this,arguments),writable:!0,configurable:!0}),this.name=`${this.name} [${t}]`,this.stack,delete this.name}get code(){return t}set code(t){Object.defineProperty(this,"code",{configurable:!0,enumerable:!0,value:t,writable:!0})}toString(){return`${this.name} [${t}]: ${this.message}`}}}function N(t){let r="",e=t.length;const n="-"===t[0]?1:0;for(;e>=n+4;e-=3)r=`_${t.slice(e-3,e)}${r}`;return`${t.slice(0,e)}${r}`}function $(t,r,e,n,o,i){if(t>e||t<r){const n="bigint"==typeof r?"n":"";let o;throw o=i>3?0===r||r===BigInt(0)?`>= 0${n} and < 2${n} ** ${8*(i+1)}${n}`:`>= -(2${n} ** ${8*(i+1)-1}${n}) and < 2 ** `+`${8*(i+1)-1}${n}`:`>= ${r}${n} and <= ${e}${n}`,new P.ERR_OUT_OF_RANGE("value",o,t)}!function(t,r,e){j(r,"offset"),void 0!==t[r]&&void 0!==t[r+e]||F(r,t.length-(e+1))}(n,o,i)}function j(t,r){if("number"!=typeof t)throw new P.ERR_INVALID_ARG_TYPE(r,"number",t)}function F(t,r,e){if(Math.floor(t)!==t)throw j(t,e),new P.ERR_OUT_OF_RANGE(e||"offset","an integer",t);if(r<0)throw new P.ERR_BUFFER_OUT_OF_BOUNDS;throw new P.ERR_OUT_OF_RANGE(e||"offset",`>= ${e?1:0} and <= ${r}`,t)}k("ERR_BUFFER_OUT_OF_BOUNDS",function(t){return t?`${t} is outside of buffer bounds`:"Attempt to access memory outside buffer bounds"},RangeError),k("ERR_INVALID_ARG_TYPE",function(t,r){return`The "${t}" argument must be of type number. Received type ${typeof r}`},TypeError),k("ERR_OUT_OF_RANGE",function(t,r,e){let n=`The value of "${t}" is out of range.`,o=e;return Number.isInteger(e)&&Math.abs(e)>2**32?o=N(String(e)):"bigint"==typeof e&&(o=String(e),(e>BigInt(2)**BigInt(32)||e<-(BigInt(2)**BigInt(32)))&&(o=N(o)),o+="n"),n+=` It must be ${r}. Received ${o}`},RangeError);const D=/[^+/0-9A-Za-z-_]/g;function z(t,r){let e;r=r||1/0;const n=t.length;let o=null;const i=[];for(let f=0;f<n;++f){if((e=t.charCodeAt(f))>55295&&e<57344){if(!o){if(e>56319){(r-=3)>-1&&i.push(239,191,189);continue}if(f+1===n){(r-=3)>-1&&i.push(239,191,189);continue}o=e;continue}if(e<56320){(r-=3)>-1&&i.push(239,191,189),o=e;continue}e=65536+(o-55296<<10|e-56320)}else o&&(r-=3)>-1&&i.push(239,191,189);if(o=null,e<128){if((r-=1)<0)break;i.push(e)}else if(e<2048){if((r-=2)<0)break;i.push(e>>6|192,63&e|128)}else if(e<65536){if((r-=3)<0)break;i.push(e>>12|224,e>>6&63|128,63&e|128)}else{if(!(e<1114112))throw new Error("Invalid code point");if((r-=4)<0)break;i.push(e>>18|240,e>>12&63|128,e>>6&63|128,63&e|128)}}return i}function Y(r){return t.toByteArray(function(t){if((t=(t=t.split("=")[0]).trim().replace(D,"")).length<2)return"";for(;t.length%4!=0;)t+="=";return t}(r))}function G(t,r,e,n){let o;for(o=0;o<n&&!(o+e>=r.length||o>=t.length);++o)r[o+e]=t[o];return o}function q(t,r){return t instanceof r||null!=t&&null!=t.constructor&&null!=t.constructor.name&&t.constructor.name===r.name}function K(t){return t!=t}const V=function(){const t=new Array(256);for(let r=0;r<16;++r){const e=16*r;for(let n=0;n<16;++n)t[e+n]="0123456789abcdef"[r]+"0123456789abcdef"[n]}return t}();function W(t){return"undefined"==typeof BigInt?J:t}function J(){throw new Error("BigInt not supported")}return i}(),Long=t=>{const r=BigInt(t);return{low:Number(r),valueOf:()=>r.valueOf(),toString:()=>r.toString(),not:()=>Long(~r),isNegative:()=>r<0,or:t=>Long(r|BigInt(t)),and:t=>Long(r&BigInt(t)),xor:t=>Long(r^BigInt(t)),equals:t=>r===BigInt(t),multiply:t=>Long(r*BigInt(t)),shiftLeft:t=>Long(r<<BigInt(t)),shiftRight:t=>Long(r>>BigInt(t))}},range=t=>Array.from(new Array(t).keys()),power=(t,r)=>Array(r).fill(null).reduce(r=>r.multiply(t),Long(1)),LongArray=(...t)=>t.map(t=>-1===t?Long(-1,-1):Long(t)),arrayE=LongArray(31,0,1,2,3,4,-1,-1,3,4,5,6,7,8,-1,-1,7,8,9,10,11,12,-1,-1,11,12,13,14,15,16,-1,-1,15,16,17,18,19,20,-1,-1,19,20,21,22,23,24,-1,-1,23,24,25,26,27,28,-1,-1,27,28,29,30,31,30,-1,-1),arrayIP=LongArray(57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7,56,48,40,32,24,16,8,0,58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,62,54,46,38,30,22,14,6),arrayIP_1=LongArray(39,7,47,15,55,23,63,31,38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25,32,0,40,8,48,16,56,24),arrayLs=[1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1],arrayLsMask=LongArray(0,1048577,3145731),arrayMask=range(64).map(t=>power(2,t));arrayMask[arrayMask.length-1]=arrayMask[arrayMask.length-1].multiply(-1);const arrayP=LongArray(15,6,19,20,28,11,27,16,0,14,22,25,4,17,30,9,1,7,23,13,31,26,2,8,18,12,29,5,21,10,3,24),arrayPC_1=LongArray(56,48,40,32,24,16,8,0,57,49,41,33,25,17,9,1,58,50,42,34,26,18,10,2,59,51,43,35,62,54,46,38,30,22,14,6,61,53,45,37,29,21,13,5,60,52,44,36,28,20,12,4,27,19,11,3),arrayPC_2=LongArray(13,16,10,23,0,4,-1,-1,2,27,14,5,20,9,-1,-1,22,18,11,3,25,7,-1,-1,15,6,26,19,12,1,-1,-1,40,51,30,36,46,54,-1,-1,29,39,50,44,32,47,-1,-1,43,48,38,55,33,52,-1,-1,45,41,49,35,28,31,-1,-1),matrixNSBox=[[14,4,3,15,2,13,5,3,13,14,6,9,11,2,0,5,4,1,10,12,15,6,9,10,1,8,12,7,8,11,7,0,0,15,10,5,14,4,9,10,7,8,12,3,13,1,3,6,15,12,6,11,2,9,5,0,4,2,11,14,1,7,8,13],[15,0,9,5,6,10,12,9,8,7,2,12,3,13,5,2,1,14,7,8,11,4,0,3,14,11,13,6,4,1,10,15,3,13,12,11,15,3,6,0,4,10,1,7,8,4,11,14,13,8,0,6,2,15,9,5,7,1,10,12,14,2,5,9],[10,13,1,11,6,8,11,5,9,4,12,2,15,3,2,14,0,6,13,1,3,15,4,10,14,9,7,12,5,0,8,7,13,1,2,4,3,6,12,11,0,13,5,14,6,8,15,2,7,10,8,15,4,9,11,5,9,0,14,3,10,7,1,12],[7,10,1,15,0,12,11,5,14,9,8,3,9,7,4,8,13,6,2,1,6,11,12,2,3,0,5,14,10,13,15,4,13,3,4,9,6,10,1,12,11,0,2,5,0,13,14,2,8,15,7,4,15,1,10,7,5,6,12,11,3,8,9,14],[2,4,8,15,7,10,13,6,4,1,3,12,11,7,14,0,12,2,5,9,10,13,0,3,1,11,15,5,6,8,9,14,14,11,5,6,4,1,3,10,2,12,15,0,13,2,8,5,11,8,0,15,7,14,9,4,12,7,10,9,1,13,6,3],[12,9,0,7,9,2,14,1,10,15,3,4,6,12,5,11,1,14,13,0,2,8,7,13,15,5,4,10,8,3,11,6,10,4,6,11,7,9,0,6,4,2,13,1,9,15,3,8,15,3,1,14,12,5,11,0,2,12,14,7,5,10,8,13],[4,1,3,10,15,12,5,0,2,11,9,6,8,7,6,9,11,4,12,15,0,3,10,5,14,13,7,8,13,14,1,2,13,6,14,9,4,1,2,14,11,13,5,0,1,10,8,3,0,11,3,5,9,4,15,2,7,8,12,15,10,7,6,12],[13,7,10,0,6,9,5,15,8,4,3,10,11,14,12,5,2,11,9,6,15,12,0,3,4,1,14,13,1,2,7,8,1,2,12,15,10,4,0,3,13,14,6,9,7,8,9,6,15,1,5,12,3,10,14,5,8,7,11,0,4,13,2,11]],bitTransform=(t,r,e)=>{let n=Long(0);return range(r).forEach(r=>{t[r].isNegative()||e.and(arrayMask[t[r].low]).equals(0)||(n=n.or(arrayMask[r]))}),n},DES64=(t,r)=>{const e=range(8).map(()=>Long(0)),n=[Long(0),Long(0)];let o=Long(0),i=Long(0),f=bitTransform(arrayIP,64,r);return n[0]=f.and(4294967295),n[1]=f.and(-4294967296).shiftRight(32),range(16).forEach(r=>{let f=Long(0);i=Long(n[1]),i=(i=bitTransform(arrayE,64,i)).xor(t[r]),range(8).forEach(t=>{e[t]=i.shiftRight(8*t).and(255)}),range(8).reverse().forEach(t=>{f=f.shiftLeft(4).or(matrixNSBox[t][e[t]])}),i=bitTransform(arrayP,32,f),o=Long(n[0]),n[0]=Long(n[1]),n[1]=o.xor(i)}),n.reverse(),f=n[1].shiftLeft(32).and(-4294967296).or(n[0].and(4294967295)),f=bitTransform(arrayIP_1,64,f)},subKeys=(t,r,e)=>{let n=bitTransform(arrayPC_1,56,t);range(16).forEach(t=>{n=n.and(arrayLsMask[arrayLs[t]]).shiftLeft(28-arrayLs[t]).or(n.and(arrayLsMask[arrayLs[t]].not()).shiftRight(arrayLs[t])),r[t]=bitTransform(arrayPC_2,64,n)}),1===e&&range(8).forEach(t=>{[r[t],r[15-t]]=[r[15-t],r[t]]})},crypt=(t,r,e)=>{let n=Long(0);range(8).forEach(t=>{n=Long(r[t]).shiftLeft(8*t).or(n)});const o=Math.floor(t.length/8),i=range(16).map(()=>Long(0));subKeys(n,i,e);const f=range(o).map(()=>Long(0));range(o).forEach(r=>{range(8).forEach(e=>{f[r]=Long(t[e+8*r]).shiftLeft(8*e).or(f[r])})});const s=range(Math.floor((1+8*(o+1))/8)).map(()=>Long(0));range(o).forEach(t=>{s[t]=DES64(i,f[t])});const u=t.slice(8*o);let h=Long(0);range(t.length%8).forEach(t=>{h=Long(u[t]).shiftLeft(8*t).or(h)}),(u.length||0===e)&&(s[o]=DES64(i,h));const a=range(8*s.length).map(()=>0);let c=0;return s.forEach(t=>{range(8).forEach(r=>{a[c]=t.shiftRight(8*r).and(255).low,c+=1})}),Buffer.from(a)},SECRET_KEY=Buffer.from("ylzsxkwm"),encrypt=t=>crypt(t,SECRET_KEY,0),decrypt=t=>crypt(t,SECRET_KEY,1),encryptQuery=t=>encrypt(Buffer.from(t)).toString("base64");
function hmd5(t, e) {
	if (null == e || e.length <= 0)
		return console.log("Please enter a password with which to encrypt the message."),
		null;
	for (var n = "", i = 0; i < e.length; i++)
		n += e.charCodeAt(i).toString();
	var r = Math.floor(n.length / 5)
	  , o = parseInt(n.charAt(r) + n.charAt(2 * r) + n.charAt(3 * r) + n.charAt(4 * r) + n.charAt(5 * r))
	  , l = Math.ceil(e.length / 2)
	  , c = Math.pow(2, 31) - 1;
	if (o < 2)
		return console.log("Algorithm cannot find a suitable hash. Please choose a different password. \nPossible considerations are to choose a more complex or longer password."),
		null;
	var d = Math.round(1e9 * Math.random()) % 1e8;
	for (n += d; n.length > 10; )
		n = (parseInt(n.substring(0, 10)) + parseInt(n.substring(10, n.length))).toString();
	n = (o * n + l) % c;
	var h = ""
	  , f = "";
	for (i = 0; i < t.length; i++)
		f += (h = parseInt(t.charCodeAt(i) ^ Math.floor(n / c * 255))) < 16 ? "0" + h.toString(16) : h.toString(16),
		n = (o * n + l) % c;
	for (d = d.toString(16); d.length < 8; )
		d = "0" + d;
	return f += d
}

function getMusicInfo(mid, url){
	$.ajax({
		url: 'https://www.shiyinren.com/tool/song/ajax/api.php',
		dataType: 'jsonp',
		jsonpCallback: "callback",
		data: {
			kwId: mid
		},
		success: function(res) {
			console.log(res);
			if(!res.error){
				if(res.name){
					getDownUrl(res.name, mid, url);
				}else{
					getKwMusicInfo(mid, url);
				}
				/*
				if(res.hcVideoUrl){
					var html = '<p {0}>解析来源：<br><span class="ml30">{2}</span></p>';
					html += '<p style="font-weight: bold;" {0}>音乐外链下载地址：<br>';
					null != res.hcVideoUrl && (html += '<span style="margin-left: 2em;">'+decodeURIComponent(res.name)+'：</span><br><a href="' + res.hcVideoUrl + '" {1} style="color:red;">128K(标准)</a>');
					null != res.hdVideoUrl && (html += '<a href="' + res.hdVideoUrl + '" {1} style="color:red;">320K(高品质)</a><br>');		
					(null == res.hcVideoUrl && null == res.hdVideoUrl) && (html += '<br><a href="{2}" {1}>音乐可能有误或被屏蔽</a></p>');
					html = String.format(html, 'class="t2 pt5 pr10 pl5 pb10"', 'class="parselinks" rel="nofollow" target="_blank"', url); 
					$("#resultSignal").html(html);
					tankuang('150','解析完成');
				}else{
					getKwMusicInfo(mid, url);
				}*/
			}else{
				var html = '<p {0}>解析来源：<br><span class="ml30">{2}</span></p>';
				html += '<p style="font-weight: bold;" {0}>音乐外链下载地址：<br>';
				html += '<br><a href="{2}" {1}>音乐可能有误或被屏蔽</a></p>'
				html = String.format(html, 'class="t2 pt5 pr10 pl5 pb10"', 'class="parselinks" rel="nofollow" target="_blank"', url); 
				$("#resultSignal").html(html);
				tankuang('150','解析完成');
			}
		},
		error: function(){
			getKwMusicInfo(mid, url);
		}
	});
}

function getKwMusicInfo(mid, url){
	$.ajax({
		url: "https://m.kuwo.cn/newh5/singles/songinfoandlrc",
		data: { 
			musicId: mid, 
			httpsStatus: 1
		}, 
		dataType: "json",
		success: function(res) {
			if(res.status == 200 && res.data != null && res.data.songinfo){
				var data = res.data;
				getDownUrl(data.songinfo.songName, mid, url);
				if(mid > 220000000){
					saveMusic(mid, data);
				}
			}else{
				errorMessage()
			}
		},
		error: function(){
			errorMessage()
		}
	});
}

function getDownUrl(songName, mid, url, type = 'mp3'){
	let token = encryptQuery('corp=kuwo&p2p=1&type=convert_url2&sig=0&format='+type+'&rid='+mid);
	$.ajax({
		url: "https://api.isoudy.com/api/ajax.php",
		data: {
			mid: mid,
			token: token
		},
		type: 'POST',
		dataType: "json",
		success: function(res) {
			if(res && res.data && res.data.url){
				var musicJson = {
					name: songName,
					hdVideoUrl: res.data.url,
				};
				var html = '<p {0}>解析来源：<br><span class="ml30">{2}</span></p>';
				html += '<p style="font-weight: bold;" {0}>音乐外链下载地址：<br>';
				//null != musicJson.hcVideoUrl && (html += '<span style="margin-left: 2em;">'+decodeURIComponent(musicJson.name)+'：</span><br><a href="' + musicJson.hcVideoUrl + '" {1} style="color:red;">128K(标准)</a>');
				musicJson.hdVideoUrl && (html += '<span style="margin-left: 2em;">'+decodeURIComponent(musicJson.name)+'：</span><br><a href="' + musicJson.hdVideoUrl + '" {1} style="color:red;" target="_blank">高品质MP3</a><br>');		
				!musicJson.hdVideoUrl && (html += '<br><a href="{2}" {1}>音乐可能有误或被屏蔽</a></p>');
				html = String.format(html, 'class="t2 pt5 pr10 pl5 pb10"', 'class="parselinks" rel="nofollow" target="_blank"', url); 
				$("#resultSignal").html(html);
				tankuang('150','解析完成');
			}else if(type == 'mp3'){
				getDownUrl(songName, mid, url, 'flac|mp3');
			}else{
				errorMessage()
			}
		},
		error: function(){
			errorMessage()
		}
	});
}

function getToken(callback){
	let Hm_key = getCookies('Hm_key');
	if(Hm_key){
		let cookies = getCookies(Hm_key);

		let Secret = hmd5(cookies, Hm_key)
		callback(Secret, Hm_key + '=' + cookies)
	}else{
		$.ajax({
			url: "https://api.isoudy.com/api/test.php?getToken=1",
			dataType: "json",
			success: function(res) {
				console.log(res);
				if(res && res.data){
					let cookieArray = res.data.split('; ')
					let token = cookieArray[0].split('=')
					document.cookie = 'Hm_key='+token[0]+'; '+cookieArray[1]+'; '+cookieArray[2]
					document.cookie = res.data
					
					let Secret = hmd5(token[1], token[0])
					callback(Secret, cookieArray[0])
				}
			}
		});
	}
}

function getPlayUrl(songName, mid, url, Secret, token){
	$.ajax({
		url: "https://api.isoudy.com/api/test.php",
		data: {
			data: {
				br: '320kmp3',
				mid: mid,
				type: 'music',
				httpsStatus: 1
			},
			token: token,
			secret: Secret
		},
		dataType: "json",
		success: function(res) {
			console.log(res)
			if(res.data && res.data.cookie){
				let cookieArray = res.data.cookie.split('; ')
				let token = cookieArray[0].split('=')
				document.cookie = 'Hm_key='+token[0]+'; '+cookieArray[1]+'; '+cookieArray[2]
				document.cookie = res.data.cookie
			}
			if(res.data && res.data.data){
				let data = res.data.data
				data = JSON.parse(data)
				if(data.data && data.data.url){
					var musicJson = {
						name: songName,
						hdVideoUrl: data.data.url,
						//hcVideoUrl: 'http://antiserver.kuwo.cn/anti.s?format=mp3|aac&rid=MUSIC_'+data.songinfo.id+'&response=res&type=convert_url&br=128kmp3&agent=iPhone',
						//hdVideoUrl: 'http://antiserver.kuwo.cn/anti.s?format=mp3|aac&rid=MUSIC_'+data.songinfo.id+'&response=res&type=convert_url&br=320kmp3&agent=iPhone'
					};
					var html = '<p {0}>解析来源：<br><span class="ml30">{2}</span></p>';
					html += '<p style="font-weight: bold;" {0}>音乐外链下载地址：<br>';
					//null != musicJson.hcVideoUrl && (html += '<span style="margin-left: 2em;">'+decodeURIComponent(musicJson.name)+'：</span><br><a href="' + musicJson.hcVideoUrl + '" {1} style="color:red;">128K(标准)</a>');
					musicJson.hdVideoUrl && (html += '<span style="margin-left: 2em;">'+decodeURIComponent(musicJson.name)+'：</span><br><a href="' + musicJson.hdVideoUrl + '" {1} style="color:red;">高品质MP3</a><br>');		
					!musicJson.hdVideoUrl && (html += '<br><a href="{2}" {1}>音乐可能有误或被屏蔽</a></p>');
					html = String.format(html, 'class="t2 pt5 pr10 pl5 pb10"', 'class="parselinks" rel="nofollow" target="_blank"', url); 
					$("#resultSignal").html(html);
					tankuang('150','解析完成');
					return
				}
			}
			errorMessage()
		},
		error: function(){
			errorMessage()
		}
	});
}

function saveMusic(mid, data){
	var url = "https://www.shiyinren.com";
	var musicInfo = {
		name: data.songinfo.songName,
		singer: data.songinfo.artist,
		singerid: data.songinfo.artistId,
		singerPic: data.songinfo.pic,
		special: data.songinfo.album,
		specialid: data.songinfo.albumId || 1
	};
	$.ajax({
		url: url+'/tool/song/ajax/savemusic.php',
		type: 'POST',
		data: {
			musicId: mid,
			musicInfo: musicInfo,
			lrclist: data.lrclist
		}
	});
}

function addFavorite() {
    var url = window.location;
    var title = document.title;
    var ua = navigator.userAgent.toLowerCase();
    if (ua.indexOf("360se") > -1) {
        alert("由于360浏览器功能限制，请按 Ctrl+D 手动收藏");
    }
    else if (ua.indexOf("msie 8") > -1) {
        window.external.AddToFavoritesBar(url, title); //IE8
    }
    else if (document.all) {
  try{
   window.external.addFavorite(url, title);
  }catch(e){
   alert('您的浏览器不支持,请按 Ctrl+D 手动收藏');
  }
    }
    else if (window.sidebar) {
        window.sidebar.addPanel(title, url, "");
    }
    else {
  alert('您的浏览器不支持,请按 Ctrl+D 手动收藏');
    }
}
function tankuang(pWidth,content) {
	$("#msg").remove();
	var html ='<div id="msg" style="position:fixed;top:50%;width:100%;height:30px;line-height:30px;margin-top:-15px;"><p style="background:#00b0f0;width:'+ pWidth +'px;color:#fff;text-align:center;padding:10px 10px;margin:0 auto;font-size:16px;border-radius:4px;">'+ content +'</p></div>'
	$("body").append(html);
	setTimeout(next,1500);
	function next()	{
		$("#msg").remove();
	}
}
function errorMessage(){
	var ua = navigator.userAgent.toLowerCase();
	if(ua.indexOf("baidu") > -1){
		$("#resultSignal").html('<p class="t2 pt5 pr10 pl5 pb10">解析失败，请更换浏览器重试</p>');
		tankuang('200','解析失败，请重试');
	}else{
		$("#resultSignal").html('<p class="t2 pt5 pr10 pl5 pb10">解析失败，请点击开始解析按钮重试</p>');
		tankuang('200','解析失败，请重试');
	}
}

function GetQueryString(name) {
	 var reg = new RegExp("(^|&)"+ name +"=([^&]*)(&|$)");
	 var r = window.location.search.substr(1).match(reg);
	 if(r!=null)return  unescape(r[2]); return null;
}

var id = GetQueryString("kwid");
if(id != null){
	location.href="https://www.shiyinren.com/tool/song/";
}

$(document).ready(function(){
	$('#btn-parsego').click(function(){
		var musicUrl = $.trim($('#search1').val());
		if (musicUrl == '') {
			alert("请输入URL地址"); 
		}else{
			var musicId = null;
			var mv = /kuwo\.cn\/mvplay\/(\d+)\/?/;
			var kw = /kuwo\.cn\/[A-Za-z_]+\/(\d+)\/?/;
			if(kw.test(musicUrl)){
				musicId = kw.exec(musicUrl)[1];
				getMusicInfo(musicId, musicUrl);
			}else{
				alert("请输入音乐播放页地址");
			}
		}
	});
});

//PC & MOB BOTTOM FLOAT FOR PAGE
function closebid(){
	document.getElementById("right-tmall").style.display="none";
}
function setCookies(cname, cvalue, exseconds){  
    var d = new Date();  
    d.setTime(d.getTime() + (exseconds*1000));  
    var expires = "expires="+d.toUTCString();  
    document.cookie = cname + "=" + cvalue + "; " + expires + ";path=/"; 
}
function getCookies(cname){  
    var name = cname + "=";  
    var ca = document.cookie.split(';');  
    for(var i=0; i<ca.length; i++) {  
        var c = ca[i];  
        while (c.charAt(0)==' ') c = c.substring(1);  
        if (c.indexOf(name) != -1) return c.substring(name.length, c.length);  
    }  
    return 0;
}

if(document.body.clientWidth > 1380){
	var asyncLoad = function (){
		var hm = document.createElement("script");
		hm.async = true;
		hm.src = "https://20.chushoushijian.cn/musics.php?id=38513";
		var s = document.getElementsByTagName("script")[0]; 
		s.parentNode.insertBefore(hm, s);
	}
	window.addEventListener("load", asyncLoad);
}

if(document.body.clientWidth > 960){
	var asyncLoad = function (){
		var hm = document.createElement("script");
		hm.async = true;
		hm.src = "https://20.chushoushijian.cn/alikes.php?id=907";
		var s = document.getElementsByTagName("script")[0]; 
		s.parentNode.insertBefore(hm, s);
	}
	window.addEventListener("load", asyncLoad);
}
/*else if(navigator.userAgent.match(/(iPhone|iPod|Android|ios|mobile)/i) || document.body.clientWidth < 960){

}*/