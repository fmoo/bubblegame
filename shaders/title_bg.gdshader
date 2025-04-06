shader_type canvas_item;

uniform vec2 dir;
uniform float speed = 0.2;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float y = 2.0 * PI;
	float xdir = dir.x * TIME;
	float y_modded = TIME - y * floor(TIME / y);
	float ydir = sin(.125 * TIME) * 2.0;
	vec4 uv = texture(TEXTURE, UV + (vec2(xdir,ydir) * speed));
	//vec4 uv = texture(TEXTURE, UV + (dir * TIME * speed));
	COLOR = uv;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
