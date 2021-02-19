import rust from '@wasm-tool/rollup-plugin-rust'
import del from 'rollup-plugin-delete'

export default [
  {
    input: { testing: 'wasm/testing/Cargo.toml' },
    output: {
      dir: 'web/wwwroot/wasm',
      format: 'iife'
    },
    plugins: [
      del({ targets: 'web/wwwroot/wasm/assets/testing*' }),
      rust({
        serverPath: '/wasm/'
      }),
    ]
  },
  {
    input: { webgl: 'wasm/webgl/Cargo.toml' },
    output: {
      dir: 'web/wwwroot/wasm',
      format: 'iife'
    },
    plugins: [
      del({ targets: 'web/wwwroot/wasm/assets/webgl*' }),
      rust({
        serverPath: '/wasm/'
      }),
    ]
  },
];