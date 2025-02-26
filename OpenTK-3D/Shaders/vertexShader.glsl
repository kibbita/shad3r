#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
    FragPos = vec3(model * vec4(aPosition, 1.0));
    
    // Note: for proper lighting, you should use the normal matrix instead of just the model matrix
    // This would be: Normal = mat3(transpose(inverse(model))) * aNormal;
    // But for simple cases, this can work:
    Normal = aNormal;
}