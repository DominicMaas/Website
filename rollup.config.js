import rust from '@wasm-tool/rollup-plugin-rust'
import del from 'rollup-plugin-delete'

export default {
    input: {
        testing: 'wasm/testing/Cargo.toml',
    },
    plugins: [
        del({ targets: 'web/wwwroot/wasm/*' }),
        rust({
            serverPath: '/wasm/'
        }),
    ],
    output: {
        dir: 'web/wwwroot/wasm',
        format: 'iife'
    }
}