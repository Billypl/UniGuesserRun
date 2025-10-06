import { render } from "@testing-library/react";
import { RecenterMap } from "../components/RecenterMap";

const mockSetView = jest.fn();

jest.mock("react-leaflet", () => ({
	useMap: () => ({
		setView: mockSetView,
	}),
}));

describe("RecenterMap component test", () => {
	it("calls map.setView with provided location", () => {
		const location: [number, number] = [10.789, -15.20];
		render(<RecenterMap location={location} />);

		expect(mockSetView).toHaveBeenCalledWith([10.789, -15.20]);
})});