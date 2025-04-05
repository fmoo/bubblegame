shader_type canvas_item;

uniform float radiuse : hint_range(0.0, 512.0);
uniform float feathere : hint_range(0.0, 1.0);
uniform vec4 color : source_color;
uniform vec4 bg_color : source_color;
uniform vec2 origin;
uniform float linewidth : hint_range(0.0,100.0);

struct CircDesc {
	vec2 origin;
	float radius;
	float width;
};

uniform int origin_to_change;
uniform float time_passed;
uniform float rate = 1.0; // This is the rate at which new circles will be created

bool in_circle(vec2 pix_pos, vec2 origin_pos, float radius, float width)
{
	float dist = distance(pix_pos, origin_pos);
	if(radius - width < dist && dist < radius + width)
	{
		return true;
	}
	else{
		return false;
	}
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//base background color, every circle pixel drawn after this
	COLOR.rgba = bg_color;
	
	float time = fract(TIME * rate); // Use the fractional part of the time to create a repeating interval
	
	if (time < 1.0 / rate) {
		//time to make new circ
		vec2 position = vec2(UV.x * 512.0, UV.y * 288.0);
		vec2 origin_pos = vec2(512.0 * origin.x, 288.0 * origin.y);
		
		float rad;
		float wid;
		bool in_circ = in_circle(position, origin_pos, rad, wid);
		if(in_circ)
		{
			COLOR.rgba = color;
		}
	}
	else
	{
		//no new circ
	}
}


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
