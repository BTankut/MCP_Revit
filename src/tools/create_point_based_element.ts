import { z } from "zod";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { withRevitConnection } from "../utils/ConnectionManager.js";

export function registerCreatePointBasedElementTool(server: McpServer) {
  server.tool(
    "create_point_based_element",
    "Create one or more point-based elements in Revit such as doors, windows, or furniture. Supports batch creation with detailed parameters including family type ID, position, dimensions, and level information. All units are in millimeters (mm).",
    {
      data: z
        .array(
          z.object({
            name: z
              .string()
              .describe("Description of the element (e.g., door, window)"),
            typeId: z
              .number()
              .optional()
              .describe("The ID of the family type to create."),
            locationPoint: z
              .object({
                x: z.number().describe("X coordinate"),
                y: z.number().describe("Y coordinate"),
                z: z.number().describe("Z coordinate"),
              })
              .describe(
                "The position coordinates where the element will be placed"
              ),
            width: z.number().describe("Width of the element in mm"),
            depth: z.number().optional().describe("Depth of the element in mm"),
            height: z.number().describe("Height of the element in mm"),
            baseLevel: z.number().describe("Base level height"),
            baseOffset: z.number().describe("Offset from the base level"),
            rotation: z
              .number()
              .optional()
              .describe("Rotation angle in degrees (0-360)"),
          })
        )
        .describe("Array of point-based elements to create"),
    },
    async (args, extra) => {
      const params = args;

      try {
        const response = await withRevitConnection(async (revitClient) => {
          return await revitClient.sendCommand(
            "create_point_based_element",
            params
          );
        });

        return {
          content: [
            {
              type: "text",
              text: JSON.stringify(response, null, 2),
            },
          ],
        };
      } catch (error) {
        return {
          content: [
            {
              type: "text",
              text: `Create point-based element failed: ${
                error instanceof Error ? error.message : String(error)
              }`,
            },
          ],
        };
      }
    }
  );
}
