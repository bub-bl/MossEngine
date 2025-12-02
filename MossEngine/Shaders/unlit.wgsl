@vertex fn main_vs(@builtin(vertex_index) vid : u32) -> @builtin(position) vec4<f32> {
    if(vid == 0u)
    {
        return vec4<f32>(-0.5, -0.5, 0.0, 1.0);
    }
    else if(vid == 1u)
    {
        return vec4<f32>(0.5, -0.5, 0.0, 1.0);
    }

    return vec4<f32>(0.0, 0.5, 0.0, 1.0);
}

@fragment fn main_fs() -> @location(0) vec4<f32> {
    return vec4<f32>(1.0, 0.0, 0.0, 1.0);
}
