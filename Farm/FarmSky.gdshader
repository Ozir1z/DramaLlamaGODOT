shader_type sky;

uniform sampler2D testTexture;
uniform float stars_intensity : hint_range(-10.0, 10.0, 0.001) = -2.829;
uniform float offset_stars : hint_range(-2.0, 2.0, 0.001) = 0.083;
uniform float star_falloff : hint_range(1.0, 10.0, 0.001) = 1.79;
uniform float stars_speed : hint_range(0.0, 1.0, 0.001) = 0.2;
uniform float stars_fade_modulation : hint_range(-1.80, 1.80, 0.001) = 0.91;
uniform float stars_cutoff : hint_range(0.0, 1.0, 0.001) = 0.702;
uniform vec4 stars_sky_color : source_color = vec4(0.0, 0.11, 0.05, 1);


vec2 rotateUV(vec2 uv,vec2 pivot,float rotation) {
    float sine = sin(rotation);
    float cosine = cos(rotation);

    uv -= pivot;
    uv.x = uv.x * cosine - uv.y * sine;
    uv.y = uv.x * sine + uv.y * cosine;
    uv += pivot;

    return uv;
}

void sky() {
	vec2 skyUV1 = EYEDIR.xz / EYEDIR.y;
	vec2 skyUV = rotateUV(skyUV1, vec2(.5), PI/4.0);
	float stars_parameters = abs((EYEDIR.y * stars_intensity) - offset_stars) * step(0.0, EYEDIR.y) * star_falloff;
	vec3 stars = texture(testTexture, (skyUV * stars_parameters) + (stars_speed * (TIME / 30.0))).rgb;
	//vec3 stars = texture(testTexture, (skyUV * stars_parameters) * rotateUV(vec2(1), vec2(.5), PI/4.0)).rgb;
	//float stars_zenith = clamp(-LIGHT0_DIRECTION.y, 0.0, 1.0);
	//stars *= stars_zenith + abs(stars_zenith - stars_fade_modulation);
	//stars = step(stars_cutoff, stars);
	//stars += stars_sky_color.rgb;
	//COLOR = stars.rgb;
	
	COLOR = stars;
	
}

