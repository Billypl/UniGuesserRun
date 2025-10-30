FROM node:22-alpine AS build
WORKDIR /app
COPY . .
RUN npm ci
RUN npm run build

# serve stage using Nginx
FROM nginx:1.25-alpine
COPY --from=build /app/build /usr/share/nginx/html

# Optional: override default nginx config if needed
# COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
