use std::cell::RefCell;
use std::rc::Rc;
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::{WebGlProgram, WebGlRenderingContext, WebGlShader};

// A macro to provide `println!(..)`-style syntax for `console.log` logging.
macro_rules! log {
    ( $( $t:tt )* ) => {
        web_sys::console::log_1(&format!( $( $t )* ).into());
    }
}

fn get_canvas() -> web_sys::HtmlCanvasElement {
    let window = web_sys::window().unwrap();
    let document = window.document().unwrap();
    let canvas = document.get_element_by_id("canvas").unwrap();
    return canvas.dyn_into::<web_sys::HtmlCanvasElement>().unwrap();
}

fn request_animation_frame(f: &Closure<dyn FnMut(f32)>) {
    let window = web_sys::window().unwrap();
    window.request_animation_frame(f.as_ref().unchecked_ref())
        .expect("should register `requestAnimationFrame` OK");
}

#[wasm_bindgen(start)]
pub fn run() -> Result<(), JsValue> {
    let window = web_sys::window().unwrap();
    let document = window.document().unwrap();
    let canvas = document.get_element_by_id("canvas").unwrap();
    let canvas: web_sys::HtmlCanvasElement = canvas.dyn_into::<web_sys::HtmlCanvasElement>()?;

    let context = canvas
        .get_context("webgl")?
        .unwrap()
        .dyn_into::<WebGlRenderingContext>()?;

    let vert_shader = compile_shader(
        &context,
        WebGlRenderingContext::VERTEX_SHADER,
        r#"
        attribute vec3 position;
        uniform mat4 Pmatrix;
        uniform mat4 Vmatrix;
        uniform mat4 Mmatrix;
        attribute vec3 color;
        varying vec3 vColor;
        void main() {
            gl_Position = Pmatrix*Vmatrix*Mmatrix*vec4(position, 1.);
            vColor = color;
        }
    "#,
    )?;
    let frag_shader = compile_shader(
        &context,
        WebGlRenderingContext::FRAGMENT_SHADER,
        r#"
        precision mediump float;
        varying vec3 vColor;
        void main() {
            gl_FragColor = vec4(vColor, 1.);
        }
    "#,
    )?;
    let program = link_program(&context, &vert_shader, &frag_shader)?;
    context.use_program(Some(&program));

    log!("Linked Program!");

    let vertices: [f32; 72] = [
        -1.0 , -1.0 , -1.0 ,  1.0 , -1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 , -1.0 ,  1.0 , -1.0,
        -1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 , -1.0 ,  1.0 ,  1.0,
        -1.0 , -1.0 , -1.0 , -1.0 ,  1.0 , -1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 , -1.0 ,  1.0,
         1.0 , -1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 , -1.0 ,  1.0,
        -1.0 , -1.0 , -1.0 , -1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 ,  1.0 ,  1.0 , -1.0 , -1.0,
        -1.0 ,  1.0 , -1.0 , -1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 ,  1.0 , -1.0,
    ];

    let colors: [f32; 72] = [
        5.0,3.0,7.0, 5.0,3.0,7.0, 5.0,3.0,7.0, 5.0,3.0,7.0,
        1.0,1.0,3.0, 1.0,1.0,3.0, 1.0,1.0,3.0, 1.0,1.0,3.0,
        0.0,0.0,1.0, 0.0,0.0,1.0, 0.0,0.0,1.0, 0.0,0.0,1.0,
        1.0,0.0,0.0, 1.0,0.0,0.0, 1.0,0.0,0.0, 1.0,0.0,0.0,
        1.0,1.0,0.0, 1.0,1.0,0.0, 1.0,1.0,0.0, 1.0,1.0,0.0,
        0.0,1.0,0.0, 0.0,1.0,0.0, 0.0,1.0,0.0, 0.0,1.0,0.0
    ];

    let indices: [u16; 36] = [
        0,1,2, 0,2,3, 4,5,6, 4,6,7,
        8,9,10, 8,10,11, 12,13,14, 12,14,15,
        16,17,18, 16,18,19, 20,21,22, 20,22,23
    ];

    // Note that `Float32Array::view` is somewhat dangerous (hence the
    // `unsafe`!). This is creating a raw view into our module's
    // `WebAssembly.Memory` buffer, but if we allocate more pages for ourself
    // (aka do a memory allocation in Rust) it'll cause the buffer to change,
    // causing the `Float32Array` to be invalid.
    //
    // As a result, after `Float32Array::view` we have to be very careful not to
    // do any memory allocations before it's dropped.

    // Bind vertex buffer and place in data
    log!("Creating vertex buffer...");
    let vertex_buffer = context.create_buffer().ok_or("failed to create vertex buffer")?;
    context.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&vertex_buffer));

    unsafe {
        let vert_array = js_sys::Float32Array::view(&vertices);

        context.buffer_data_with_array_buffer_view(
            WebGlRenderingContext::ARRAY_BUFFER,
            &vert_array,
            WebGlRenderingContext::STATIC_DRAW,
        );
    }
    log!("Done!");

    // Bind color buffer and place in data
    log!("Creating color buffer...");
    let color_buffer = context.create_buffer().ok_or("failed to create color buffer")?;
    context.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&color_buffer));

    unsafe {
        let color_array = js_sys::Float32Array::view(&colors);

        context.buffer_data_with_array_buffer_view(
            WebGlRenderingContext::ARRAY_BUFFER,
            &color_array,
            WebGlRenderingContext::STATIC_DRAW,
        );
    }
    log!("Done!");

    // Bind index buffer and place in data
    log!("Creating index buffer...");
    let index_buffer = context.create_buffer().ok_or("failed to create index buffer")?;
    context.bind_buffer(WebGlRenderingContext::ELEMENT_ARRAY_BUFFER, Some(&index_buffer));

    unsafe {
        let index_array = js_sys::Uint16Array::view(&indices);
        context.buffer_data_with_array_buffer_view(
            WebGlRenderingContext::ELEMENT_ARRAY_BUFFER,
            &index_array,
            WebGlRenderingContext::STATIC_DRAW,
        );
    }
    log!("Done!");

    // Associating attributes to vertex shader
    log!("Associating attributes to vertex shader");
    let p_matrix = context.get_uniform_location(&program, "Pmatrix").ok_or("failed to find p_matrix uniform location")?;
    let v_matrix = context.get_uniform_location(&program, "Vmatrix").ok_or("failed to find v_matrix uniform location")?;
    let m_matrix = context.get_uniform_location(&program, "Mmatrix").ok_or("failed to find m_matrix uniform location")?;

    // position buffer
    log!("Binding Position Buffer...");
    context.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&vertex_buffer));
    let _position = context.get_attrib_location(&program, "position") as u32;

    context.vertex_attrib_pointer_with_i32(_position, 3, WebGlRenderingContext::FLOAT, false, 0, 0);
    context.enable_vertex_attrib_array(_position);

    // color buffer
    log!("Binding Color Buffer...");
    context.bind_buffer(WebGlRenderingContext::ARRAY_BUFFER, Some(&color_buffer));
    let _color = context.get_attrib_location(&program, "color") as u32;

    context.vertex_attrib_pointer_with_i32(_color, 3, WebGlRenderingContext::FLOAT, false, 0, 0);
    context.enable_vertex_attrib_array(_color);

    // Matrices
    log!("Matrices");
    let mut proj_matrix: [f32; 16] = get_projection_matrix(40.0, canvas.width() as f32 / canvas.height() as f32, 1.0, 10.0);
    let mut mo_matrix: [f32; 16] = [ 1.0,0.0,0.0,0.0, 0.0,1.0,0.0,0.0, 0.0,0.0,1.0,0.0, 0.0,0.0,0.0,1.0 ];
    let mut view_matrix: [f32; 16] = [ 1.0,0.0,0.0,0.0, 0.0,1.0,0.0,0.0, 0.0,0.0,1.0,0.0, 0.0,0.0,0.0,1.0 ];

    view_matrix[14] = view_matrix[14] - 6.0;

    // Mouse events
    let amortization: f32 = 0.95;
    let mut drag = false;
    let mut old_x: f32 = 0.0;
    let mut old_y: f32 = 0.0;
    let mut d_x: f32 = 0.0;
    let mut d_y: f32 = 0.0;

    let mut theta: f32 = 0.0;
    let mut phi: f32 = 0.0;
    let mut time_old: f32 = 0.0;

    let mouse_down = Closure::wrap(Box::new(move |event: web_sys::MouseEvent| {
        drag = true;

        old_x = event.page_x() as f32;
        old_y = event.page_y() as f32;

        event.prevent_default();

        log!("Mouse DOWN");
        return false;
    }) as Box<dyn FnMut(_) -> bool>);

    let mouse_up = Closure::wrap(Box::new(move |event: web_sys::MouseEvent| {
        drag = false;
    }) as Box<dyn FnMut(_)>);

    let mouse_move = Closure::wrap(Box::new(move |event: web_sys::MouseEvent| {
        if drag == false { return false; }

        let c = get_canvas();
        let canvas_width = c.width();
        let canvas_height = c.height();

        d_x = (event.page_x() as f32 - old_x) * 2.0 * std::f32::consts::PI / canvas_width as f32;
        d_y = (event.page_y() as f32 - old_y) * 2.0 * std::f32::consts::PI  / canvas_height as f32;

        theta += d_x;
        phi += d_y;

        old_x = event.page_x() as f32;
        old_y = event.page_y() as f32;

        event.prevent_default();

        return false;
    }) as Box<dyn FnMut(_) -> bool>);

    // Bind out actual events
    canvas.add_event_listener_with_callback("mousedown", mouse_down.as_ref().unchecked_ref());
    canvas.add_event_listener_with_callback("mouseup", mouse_up.as_ref().unchecked_ref());
    canvas.add_event_listener_with_callback("mouseout", mouse_up.as_ref().unchecked_ref());
    canvas.add_event_listener_with_callback("mousemove", mouse_move.as_ref().unchecked_ref());

    // Forget these closures, this treats these closures as global, technically
    // leaks memory, but we don't really care
    mouse_down.forget();
    mouse_up.forget();
    mouse_move.forget();

    // Drawing

    let f = Rc::new(RefCell::new(None));
    let g = f.clone();

    *g.borrow_mut()  = Some(Closure::wrap(Box::new(move |time: f32| {
        let dt = time - time_old;

        if !drag {
            d_x *= amortization;
            d_y *= amortization;

            theta += d_x;
            phi += d_y;
        }

        //set model matrix to I4

        mo_matrix[0] = 1.0; mo_matrix[1] = 0.0; mo_matrix[2] = 0.0;
        mo_matrix[3] = 0.0;

        mo_matrix[4] = 0.0; mo_matrix[5] = 1.0; mo_matrix[6] = 0.0;
        mo_matrix[7] = 0.0;

        mo_matrix[8] = 0.0; mo_matrix[9] = 0.0; mo_matrix[10] = 1.0;
        mo_matrix[11] = 0.0;

        mo_matrix[12] = 0.0; mo_matrix[13] = 0.0; mo_matrix[14] = 0.0;
        mo_matrix[15] = 1.0;

        rotate_y(mo_matrix, theta);
        rotate_x(mo_matrix, phi);

        time_old = time;

        context.enable(WebGlRenderingContext::DEPTH_TEST);

        context.clear_color(0.0, 0.0, 0.0, 0.0);
        context.clear_depth(1.0);

        let c = get_canvas();
        context.viewport(0, 0, c.width() as i32, c.height() as i32);
        context.clear(WebGlRenderingContext::COLOR_BUFFER_BIT | WebGlRenderingContext::DEPTH_BUFFER_BIT);

        context.uniform_matrix4fv_with_f32_array(Some(&p_matrix), false, &proj_matrix);
        context.uniform_matrix4fv_with_f32_array(Some(&v_matrix), false, &view_matrix);
        context.uniform_matrix4fv_with_f32_array(Some(&m_matrix), false, &mo_matrix);

        context.bind_buffer(WebGlRenderingContext::ELEMENT_ARRAY_BUFFER, Some(&index_buffer));
        context.draw_elements_with_i32(WebGlRenderingContext::TRIANGLES, indices.len() as i32, WebGlRenderingContext::UNSIGNED_SHORT, 0);

        request_animation_frame(f.borrow().as_ref().unwrap());
    }) as Box<dyn FnMut(f32)>));

    request_animation_frame(g.borrow().as_ref().unwrap());

    //request_animation_frame(&animate);

    Ok(())
}

pub fn compile_shader(
    context: &WebGlRenderingContext,
    shader_type: u32,
    source: &str,
) -> Result<WebGlShader, String> {
    let shader = context
        .create_shader(shader_type)
        .ok_or_else(|| String::from("Unable to create shader object"))?;
    context.shader_source(&shader, source);
    context.compile_shader(&shader);

    if context
        .get_shader_parameter(&shader, WebGlRenderingContext::COMPILE_STATUS)
        .as_bool()
        .unwrap_or(false)
    {
        Ok(shader)
    } else {
        Err(context
            .get_shader_info_log(&shader)
            .unwrap_or_else(|| String::from("Unknown error creating shader")))
    }
}

pub fn link_program(
    context: &WebGlRenderingContext,
    vert_shader: &WebGlShader,
    frag_shader: &WebGlShader,
) -> Result<WebGlProgram, String> {
    let program = context
        .create_program()
        .ok_or_else(|| String::from("Unable to create shader object"))?;

    context.attach_shader(&program, vert_shader);
    context.attach_shader(&program, frag_shader);
    context.link_program(&program);

    if context
        .get_program_parameter(&program, WebGlRenderingContext::LINK_STATUS)
        .as_bool()
        .unwrap_or(false)
    {
        Ok(program)
    } else {
        Err(context
            .get_program_info_log(&program)
            .unwrap_or_else(|| String::from("Unknown error creating program object")))
    }
}

fn get_projection_matrix(angle: f32, a: f32, z_min: f32, z_max: f32) -> [f32; 16] {
    let ang = ((angle * 0.5) * std::f32::consts::PI/180.0).tan();

    return [
        0.5 / ang , 0.0           , 0.0                                      ,  0.0,
        0.0       , 0.5 * a / ang , 0.0                                      ,  0.0,
        0.0       , 0.0           , -(z_max + z_min) / (z_max - z_min)       , -1.0,
        0.0       , 0.0           , (-2.0 * z_max * z_min) / (z_max - z_min) ,  0.0
    ];
}

fn rotate_x(mut m: [f32; 16], angle: f32) {
    let c = angle.cos();
    let s = angle.sin();
    let mv1 = m[1];
    let mv5 = m[5];
    let mv9 = m[9];

    m[1] = m[1]   * c - m[2]  * s;
    m[5] = m[5]   * c - m[6]  * s;
    m[9] = m[9]   * c - m[10] * s;

    m[2] = m[2]   * c + mv1   * s;
    m[6] = m[6]   * c + mv5   * s;
    m[10] = m[10] * c + mv9   * s;
}

fn rotate_y(mut m: [f32; 16], angle: f32)
{
    let c = angle.cos();
    let s = angle.sin();
    let mv0 = m[0];
    let mv4 = m[4];
    let mv8 = m[8];

    m[0] = c  * m[0]  + s * m[2];
    m[4] = c  * m[4]  + s * m[6];
    m[8] = c  * m[8]  + s * m[10];

    m[2] = c  * m[2]  - s * mv0;
    m[6] = c  * m[6]  - s * mv4;
    m[10] = c * m[10] - s * mv8;
}