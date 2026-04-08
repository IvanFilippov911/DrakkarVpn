FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .

ARG VITE_API_BASE=/api
ENV VITE_API_BASE=${VITE_API_BASE}
ARG VITE_HAPP_ANDROID_URL=
ARG VITE_HAPP_IOS_URL=
ARG VITE_HAPP_MACOS_URL=
ENV VITE_HAPP_ANDROID_URL=${VITE_HAPP_ANDROID_URL}
ENV VITE_HAPP_IOS_URL=${VITE_HAPP_IOS_URL}
ENV VITE_HAPP_MACOS_URL=${VITE_HAPP_MACOS_URL}

RUN npm run build

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/dist ./

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]