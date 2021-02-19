import rust from '@wasm-tool/rollup-plugin-rust'

export default {
    input: {
        testing: 'wasm/testing/Cargo.toml',
    },
    plugins: [
        rust({
            serverPath: '/wasm/'
        }),
    ],
    output: {
        dir: 'web/wwwroot/wasm',
        format: 'iife'
    }
}