(function () {
    'use strict';

    let wasm;

    const heap = new Array(32).fill(undefined);

    heap.push(undefined, null, true, false);

    function getObject(idx) { return heap[idx]; }

    let heap_next = heap.length;

    function dropObject(idx) {
        if (idx < 36) return;
        heap[idx] = heap_next;
        heap_next = idx;
    }

    function takeObject(idx) {
        const ret = getObject(idx);
        dropObject(idx);
        return ret;
    }

    let cachedTextDecoder = new TextDecoder('utf-8', { ignoreBOM: true, fatal: true });

    cachedTextDecoder.decode();

    let cachegetUint8Memory0 = null;
    function getUint8Memory0() {
        if (cachegetUint8Memory0 === null || cachegetUint8Memory0.buffer !== wasm.memory.buffer) {
            cachegetUint8Memory0 = new Uint8Array(wasm.memory.buffer);
        }
        return cachegetUint8Memory0;
    }

    function getStringFromWasm0(ptr, len) {
        return cachedTextDecoder.decode(getUint8Memory0().subarray(ptr, ptr + len));
    }

    function addHeapObject(obj) {
        if (heap_next === heap.length) heap.push(heap.length + 1);
        const idx = heap_next;
        heap_next = heap[idx];

        heap[idx] = obj;
        return idx;
    }

    function debugString(val) {
        // primitive types
        const type = typeof val;
        if (type == 'number' || type == 'boolean' || val == null) {
            return  `${val}`;
        }
        if (type == 'string') {
            return `"${val}"`;
        }
        if (type == 'symbol') {
            const description = val.description;
            if (description == null) {
                return 'Symbol';
            } else {
                return `Symbol(${description})`;
            }
        }
        if (type == 'function') {
            const name = val.name;
            if (typeof name == 'string' && name.length > 0) {
                return `Function(${name})`;
            } else {
                return 'Function';
            }
        }
        // objects
        if (Array.isArray(val)) {
            const length = val.length;
            let debug = '[';
            if (length > 0) {
                debug += debugString(val[0]);
            }
            for(let i = 1; i < length; i++) {
                debug += ', ' + debugString(val[i]);
            }
            debug += ']';
            return debug;
        }
        // Test for built-in
        const builtInMatches = /\[object ([^\]]+)\]/.exec(toString.call(val));
        let className;
        if (builtInMatches.length > 1) {
            className = builtInMatches[1];
        } else {
            // Failed to match the standard '[object ClassName]'
            return toString.call(val);
        }
        if (className == 'Object') {
            // we're a user defined class or Object
            // JSON.stringify avoids problems with cycles, and is generally much
            // easier than looping through ownProperties of `val`.
            try {
                return 'Object(' + JSON.stringify(val) + ')';
            } catch (_) {
                return 'Object';
            }
        }
        // errors
        if (val instanceof Error) {
            return `${val.name}: ${val.message}\n${val.stack}`;
        }
        // TODO we could test for more things here, like `Set`s and `Map`s.
        return className;
    }

    let WASM_VECTOR_LEN = 0;

    let cachedTextEncoder = new TextEncoder('utf-8');

    const encodeString = (typeof cachedTextEncoder.encodeInto === 'function'
        ? function (arg, view) {
        return cachedTextEncoder.encodeInto(arg, view);
    }
        : function (arg, view) {
        const buf = cachedTextEncoder.encode(arg);
        view.set(buf);
        return {
            read: arg.length,
            written: buf.length
        };
    });

    function passStringToWasm0(arg, malloc, realloc) {

        if (realloc === undefined) {
            const buf = cachedTextEncoder.encode(arg);
            const ptr = malloc(buf.length);
            getUint8Memory0().subarray(ptr, ptr + buf.length).set(buf);
            WASM_VECTOR_LEN = buf.length;
            return ptr;
        }

        let len = arg.length;
        let ptr = malloc(len);

        const mem = getUint8Memory0();

        let offset = 0;

        for (; offset < len; offset++) {
            const code = arg.charCodeAt(offset);
            if (code > 0x7F) break;
            mem[ptr + offset] = code;
        }

        if (offset !== len) {
            if (offset !== 0) {
                arg = arg.slice(offset);
            }
            ptr = realloc(ptr, len, len = offset + arg.length * 3);
            const view = getUint8Memory0().subarray(ptr + offset, ptr + len);
            const ret = encodeString(arg, view);

            offset += ret.written;
        }

        WASM_VECTOR_LEN = offset;
        return ptr;
    }

    let cachegetInt32Memory0 = null;
    function getInt32Memory0() {
        if (cachegetInt32Memory0 === null || cachegetInt32Memory0.buffer !== wasm.memory.buffer) {
            cachegetInt32Memory0 = new Int32Array(wasm.memory.buffer);
        }
        return cachegetInt32Memory0;
    }

    function makeMutClosure(arg0, arg1, dtor, f) {
        const state = { a: arg0, b: arg1, cnt: 1, dtor };
        const real = (...args) => {
            // First up with a closure we increment the internal reference
            // count. This ensures that the Rust closure environment won't
            // be deallocated while we're invoking it.
            state.cnt++;
            const a = state.a;
            state.a = 0;
            try {
                return f(a, state.b, ...args);
            } finally {
                if (--state.cnt === 0) {
                    wasm.__wbindgen_export_2.get(state.dtor)(a, state.b);

                } else {
                    state.a = a;
                }
            }
        };
        real.original = state;

        return real;
    }
    function __wbg_adapter_22(arg0, arg1, arg2) {
        var ret = wasm._dyn_core__ops__function__FnMut__A____Output___R_as_wasm_bindgen__closure__WasmClosure___describe__invoke__hc24e90fc81ffb44f(arg0, arg1, addHeapObject(arg2));
        return ret !== 0;
    }

    function __wbg_adapter_25(arg0, arg1, arg2) {
        wasm._dyn_core__ops__function__FnMut__A____Output___R_as_wasm_bindgen__closure__WasmClosure___describe__invoke__hca6acf415b318b2d(arg0, arg1, addHeapObject(arg2));
    }

    function __wbg_adapter_28(arg0, arg1, arg2) {
        wasm._dyn_core__ops__function__FnMut__A____Output___R_as_wasm_bindgen__closure__WasmClosure___describe__invoke__h4df9e8eb665529b2(arg0, arg1, arg2);
    }

    function isLikeNone(x) {
        return x === undefined || x === null;
    }

    function handleError(f, args) {
        try {
            return f.apply(this, args);
        } catch (e) {
            wasm.__wbindgen_exn_store(addHeapObject(e));
        }
    }

    let cachegetFloat32Memory0 = null;
    function getFloat32Memory0() {
        if (cachegetFloat32Memory0 === null || cachegetFloat32Memory0.buffer !== wasm.memory.buffer) {
            cachegetFloat32Memory0 = new Float32Array(wasm.memory.buffer);
        }
        return cachegetFloat32Memory0;
    }

    function getArrayF32FromWasm0(ptr, len) {
        return getFloat32Memory0().subarray(ptr / 4, ptr / 4 + len);
    }

    async function load(module, imports) {
        if (typeof Response === 'function' && module instanceof Response) {
            if (typeof WebAssembly.instantiateStreaming === 'function') {
                try {
                    return await WebAssembly.instantiateStreaming(module, imports);

                } catch (e) {
                    if (module.headers.get('Content-Type') != 'application/wasm') {
                        console.warn("`WebAssembly.instantiateStreaming` failed because your server does not serve wasm with `application/wasm` MIME type. Falling back to `WebAssembly.instantiate` which is slower. Original error:\n", e);

                    } else {
                        throw e;
                    }
                }
            }

            const bytes = await module.arrayBuffer();
            return await WebAssembly.instantiate(bytes, imports);

        } else {
            const instance = await WebAssembly.instantiate(module, imports);

            if (instance instanceof WebAssembly.Instance) {
                return { instance, module };

            } else {
                return instance;
            }
        }
    }

    async function init(input) {
        if (typeof input === 'undefined') {
            input = new URL('index_bg.wasm', (document.currentScript && document.currentScript.src || new URL('webgl.js', document.baseURI).href));
        }
        const imports = {};
        imports.wbg = {};
        imports.wbg.__wbindgen_object_drop_ref = function(arg0) {
            takeObject(arg0);
        };
        imports.wbg.__wbindgen_cb_drop = function(arg0) {
            const obj = takeObject(arg0).original;
            if (obj.cnt-- == 1) {
                obj.a = 0;
                return true;
            }
            var ret = false;
            return ret;
        };
        imports.wbg.__wbindgen_string_new = function(arg0, arg1) {
            var ret = getStringFromWasm0(arg0, arg1);
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_instanceof_Window_11e25482011fc506 = function(arg0) {
            var ret = getObject(arg0) instanceof Window;
            return ret;
        };
        imports.wbg.__wbg_document_5aff8cd83ef968f5 = function(arg0) {
            var ret = getObject(arg0).document;
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_requestAnimationFrame_1fb079d39e1b8a26 = function() { return handleError(function (arg0, arg1) {
            var ret = getObject(arg0).requestAnimationFrame(getObject(arg1));
            return ret;
        }, arguments) };
        imports.wbg.__wbg_getElementById_b180ea4ada06a837 = function(arg0, arg1, arg2) {
            var ret = getObject(arg0).getElementById(getStringFromWasm0(arg1, arg2));
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_pageX_6959a4d2251be7a5 = function(arg0) {
            var ret = getObject(arg0).pageX;
            return ret;
        };
        imports.wbg.__wbg_pageY_2cd18972c38345bc = function(arg0) {
            var ret = getObject(arg0).pageY;
            return ret;
        };
        imports.wbg.__wbg_addEventListener_936431894dca4639 = function() { return handleError(function (arg0, arg1, arg2, arg3) {
            getObject(arg0).addEventListener(getStringFromWasm0(arg1, arg2), getObject(arg3));
        }, arguments) };
        imports.wbg.__wbg_instanceof_WebGlRenderingContext_c86a7d34366b6a22 = function(arg0) {
            var ret = getObject(arg0) instanceof WebGLRenderingContext;
            return ret;
        };
        imports.wbg.__wbg_bufferData_fc1c7f6f7937aa2f = function(arg0, arg1, arg2, arg3) {
            getObject(arg0).bufferData(arg1 >>> 0, getObject(arg2), arg3 >>> 0);
        };
        imports.wbg.__wbg_uniformMatrix4fv_ec627ec788c1b744 = function(arg0, arg1, arg2, arg3, arg4) {
            getObject(arg0).uniformMatrix4fv(getObject(arg1), arg2 !== 0, getArrayF32FromWasm0(arg3, arg4));
        };
        imports.wbg.__wbg_attachShader_6124f72095cdcf11 = function(arg0, arg1, arg2) {
            getObject(arg0).attachShader(getObject(arg1), getObject(arg2));
        };
        imports.wbg.__wbg_bindBuffer_275d909129fba2de = function(arg0, arg1, arg2) {
            getObject(arg0).bindBuffer(arg1 >>> 0, getObject(arg2));
        };
        imports.wbg.__wbg_clear_65a182ed82b4f282 = function(arg0, arg1) {
            getObject(arg0).clear(arg1 >>> 0);
        };
        imports.wbg.__wbg_clearColor_e0034d2b65202787 = function(arg0, arg1, arg2, arg3, arg4) {
            getObject(arg0).clearColor(arg1, arg2, arg3, arg4);
        };
        imports.wbg.__wbg_clearDepth_c6f6872b2c7a7d3e = function(arg0, arg1) {
            getObject(arg0).clearDepth(arg1);
        };
        imports.wbg.__wbg_compileShader_42fdaee532cdb8e4 = function(arg0, arg1) {
            getObject(arg0).compileShader(getObject(arg1));
        };
        imports.wbg.__wbg_createBuffer_3691dcedc890b4e8 = function(arg0) {
            var ret = getObject(arg0).createBuffer();
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_createProgram_8edfd62e0586640d = function(arg0) {
            var ret = getObject(arg0).createProgram();
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_createShader_7033c38612c5688d = function(arg0, arg1) {
            var ret = getObject(arg0).createShader(arg1 >>> 0);
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_drawElements_4ec52596f5751396 = function(arg0, arg1, arg2, arg3, arg4) {
            getObject(arg0).drawElements(arg1 >>> 0, arg2, arg3 >>> 0, arg4);
        };
        imports.wbg.__wbg_enable_8f92d01df1a4c77c = function(arg0, arg1) {
            getObject(arg0).enable(arg1 >>> 0);
        };
        imports.wbg.__wbg_enableVertexAttribArray_4b127e0ecccd536c = function(arg0, arg1) {
            getObject(arg0).enableVertexAttribArray(arg1 >>> 0);
        };
        imports.wbg.__wbg_getAttribLocation_e3a341ce3579c6e6 = function(arg0, arg1, arg2, arg3) {
            var ret = getObject(arg0).getAttribLocation(getObject(arg1), getStringFromWasm0(arg2, arg3));
            return ret;
        };
        imports.wbg.__wbg_getProgramInfoLog_0b10742df7a2ebea = function(arg0, arg1, arg2) {
            var ret = getObject(arg1).getProgramInfoLog(getObject(arg2));
            var ptr0 = isLikeNone(ret) ? 0 : passStringToWasm0(ret, wasm.__wbindgen_malloc, wasm.__wbindgen_realloc);
            var len0 = WASM_VECTOR_LEN;
            getInt32Memory0()[arg0 / 4 + 1] = len0;
            getInt32Memory0()[arg0 / 4 + 0] = ptr0;
        };
        imports.wbg.__wbg_getProgramParameter_bb277a1d000dd7a1 = function(arg0, arg1, arg2) {
            var ret = getObject(arg0).getProgramParameter(getObject(arg1), arg2 >>> 0);
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_getShaderInfoLog_950ab0c3fc7afa37 = function(arg0, arg1, arg2) {
            var ret = getObject(arg1).getShaderInfoLog(getObject(arg2));
            var ptr0 = isLikeNone(ret) ? 0 : passStringToWasm0(ret, wasm.__wbindgen_malloc, wasm.__wbindgen_realloc);
            var len0 = WASM_VECTOR_LEN;
            getInt32Memory0()[arg0 / 4 + 1] = len0;
            getInt32Memory0()[arg0 / 4 + 0] = ptr0;
        };
        imports.wbg.__wbg_getShaderParameter_54891c5545a79869 = function(arg0, arg1, arg2) {
            var ret = getObject(arg0).getShaderParameter(getObject(arg1), arg2 >>> 0);
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_getUniformLocation_8b0d07c81923dc0a = function(arg0, arg1, arg2, arg3) {
            var ret = getObject(arg0).getUniformLocation(getObject(arg1), getStringFromWasm0(arg2, arg3));
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        };
        imports.wbg.__wbg_linkProgram_0be4bd888f743eb0 = function(arg0, arg1) {
            getObject(arg0).linkProgram(getObject(arg1));
        };
        imports.wbg.__wbg_shaderSource_c666880b620c8f34 = function(arg0, arg1, arg2, arg3) {
            getObject(arg0).shaderSource(getObject(arg1), getStringFromWasm0(arg2, arg3));
        };
        imports.wbg.__wbg_useProgram_fb4984fb080bcd61 = function(arg0, arg1) {
            getObject(arg0).useProgram(getObject(arg1));
        };
        imports.wbg.__wbg_vertexAttribPointer_d00262e9bf7a3742 = function(arg0, arg1, arg2, arg3, arg4, arg5, arg6) {
            getObject(arg0).vertexAttribPointer(arg1 >>> 0, arg2, arg3 >>> 0, arg4 !== 0, arg5, arg6);
        };
        imports.wbg.__wbg_viewport_39356c8cdec98b8b = function(arg0, arg1, arg2, arg3, arg4) {
            getObject(arg0).viewport(arg1, arg2, arg3, arg4);
        };
        imports.wbg.__wbg_log_9a99fb1af846153b = function(arg0) {
            console.log(getObject(arg0));
        };
        imports.wbg.__wbg_instanceof_HtmlCanvasElement_fd3cbbe3906d7792 = function(arg0) {
            var ret = getObject(arg0) instanceof HTMLCanvasElement;
            return ret;
        };
        imports.wbg.__wbg_width_9eb2c66ac9dde633 = function(arg0) {
            var ret = getObject(arg0).width;
            return ret;
        };
        imports.wbg.__wbg_height_64e5d4222af3fb90 = function(arg0) {
            var ret = getObject(arg0).height;
            return ret;
        };
        imports.wbg.__wbg_getContext_813df131fcbd6e91 = function() { return handleError(function (arg0, arg1, arg2) {
            var ret = getObject(arg0).getContext(getStringFromWasm0(arg1, arg2));
            return isLikeNone(ret) ? 0 : addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbg_preventDefault_7c4d18eb2bb1a26a = function(arg0) {
            getObject(arg0).preventDefault();
        };
        imports.wbg.__wbg_call_ba36642bd901572b = function() { return handleError(function (arg0, arg1) {
            var ret = getObject(arg0).call(getObject(arg1));
            return addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbindgen_object_clone_ref = function(arg0) {
            var ret = getObject(arg0);
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_newnoargs_9fdd8f3961dd1bee = function(arg0, arg1) {
            var ret = new Function(getStringFromWasm0(arg0, arg1));
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_self_bb69a836a72ec6e9 = function() { return handleError(function () {
            var ret = self.self;
            return addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbg_window_3304fc4b414c9693 = function() { return handleError(function () {
            var ret = window.window;
            return addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbg_globalThis_e0d21cabc6630763 = function() { return handleError(function () {
            var ret = globalThis.globalThis;
            return addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbg_global_8463719227271676 = function() { return handleError(function () {
            var ret = global.global;
            return addHeapObject(ret);
        }, arguments) };
        imports.wbg.__wbindgen_is_undefined = function(arg0) {
            var ret = getObject(arg0) === undefined;
            return ret;
        };
        imports.wbg.__wbg_buffer_9e184d6f785de5ed = function(arg0) {
            var ret = getObject(arg0).buffer;
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_newwithbyteoffsetandlength_9f0558f3c1740b0a = function(arg0, arg1, arg2) {
            var ret = new Uint16Array(getObject(arg0), arg1 >>> 0, arg2 >>> 0);
            return addHeapObject(ret);
        };
        imports.wbg.__wbg_newwithbyteoffsetandlength_abfc24c57fd08e6d = function(arg0, arg1, arg2) {
            var ret = new Float32Array(getObject(arg0), arg1 >>> 0, arg2 >>> 0);
            return addHeapObject(ret);
        };
        imports.wbg.__wbindgen_boolean_get = function(arg0) {
            const v = getObject(arg0);
            var ret = typeof(v) === 'boolean' ? (v ? 1 : 0) : 2;
            return ret;
        };
        imports.wbg.__wbindgen_debug_string = function(arg0, arg1) {
            var ret = debugString(getObject(arg1));
            var ptr0 = passStringToWasm0(ret, wasm.__wbindgen_malloc, wasm.__wbindgen_realloc);
            var len0 = WASM_VECTOR_LEN;
            getInt32Memory0()[arg0 / 4 + 1] = len0;
            getInt32Memory0()[arg0 / 4 + 0] = ptr0;
        };
        imports.wbg.__wbindgen_throw = function(arg0, arg1) {
            throw new Error(getStringFromWasm0(arg0, arg1));
        };
        imports.wbg.__wbindgen_rethrow = function(arg0) {
            throw takeObject(arg0);
        };
        imports.wbg.__wbindgen_memory = function() {
            var ret = wasm.memory;
            return addHeapObject(ret);
        };
        imports.wbg.__wbindgen_closure_wrapper77 = function(arg0, arg1, arg2) {
            var ret = makeMutClosure(arg0, arg1, 18, __wbg_adapter_22);
            return addHeapObject(ret);
        };
        imports.wbg.__wbindgen_closure_wrapper79 = function(arg0, arg1, arg2) {
            var ret = makeMutClosure(arg0, arg1, 18, __wbg_adapter_25);
            return addHeapObject(ret);
        };
        imports.wbg.__wbindgen_closure_wrapper81 = function(arg0, arg1, arg2) {
            var ret = makeMutClosure(arg0, arg1, 18, __wbg_adapter_28);
            return addHeapObject(ret);
        };

        if (typeof input === 'string' || (typeof Request === 'function' && input instanceof Request) || (typeof URL === 'function' && input instanceof URL)) {
            input = fetch(input);
        }



        const { instance, module } = await load(await input, imports);

        wasm = instance.exports;
        init.__wbindgen_wasm_module = module;
        wasm.__wbindgen_start();
        return wasm;
    }

    init("/wasm/assets/webgl-153b6f20.wasm").catch(console.error);

}());
