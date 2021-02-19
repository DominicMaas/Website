use wasm_bindgen::prelude::*;

#[wasm_bindgen]
extern {
    pub fn alert(s: &str);
}

#[wasm_bindgen(start)]
pub fn run() {
    alert("Hello world from WASM!");
}